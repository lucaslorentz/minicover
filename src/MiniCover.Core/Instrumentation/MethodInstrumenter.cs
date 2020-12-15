using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using MiniCover.Core.Extensions;
using MiniCover.Core.FileSystem;
using MiniCover.Core.Instrumentation.Patterns;
using MiniCover.Core.Model;
using MiniCover.HitServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace MiniCover.Core.Instrumentation
{
    public class MethodInstrumenter
    {
        private static readonly Type hitServiceType = typeof(HitService);
        private static readonly Type methodScopeType = typeof(MethodScope);
        private static readonly MethodInfo hitMethodInfo = methodScopeType.GetMethod(nameof(MethodScope.Hit));
        private static readonly MethodInfo enterMethodInfo = hitServiceType.GetMethod(nameof(HitService.EnterMethod));
        private static readonly MethodInfo disposeMethodInfo = methodScopeType.GetMethod(nameof(IDisposable.Dispose));

        private readonly ILogger<MethodInstrumenter> _logger;
        private readonly IFileReader _fileReader;

        public MethodInstrumenter(
            ILogger<MethodInstrumenter> logger,
            IFileReader fileReader)
        {
            _logger = logger;
            _fileReader = fileReader;
        }

        public void InstrumentMethod(
            InstrumentationContext context,
            bool instrumentInstructions,
            MethodDefinition methodDefinition,
            InstrumentedAssembly instrumentedAssembly)
        {
            var originalMethod = methodDefinition.ResolveOriginalMethod();

            var instrumentedMethod = instrumentedAssembly.GetOrAddMethod(
                originalMethod.DeclaringType.FullName,
                originalMethod.Name,
                originalMethod.FullName
            );

            var methodContextClassReference = methodDefinition.Module.GetOrImportReference(methodScopeType);
            var enterMethodReference = methodDefinition.Module.GetOrImportReference(enterMethodInfo);
            var disposeMethodReference = methodDefinition.Module.GetOrImportReference(disposeMethodInfo);

            var sequencePointsInstructions = methodDefinition.MapSequencePointsToInstructions().ToArray();

            var ilProcessor = methodDefinition.Body.GetILProcessor();
            ilProcessor.Body.InitLocals = true;
            ilProcessor.Body.SimplifyMacros();

            var methodContextVariable = new VariableDefinition(methodContextClassReference);
            ilProcessor.Body.Variables.Add(methodContextVariable);

            ilProcessor.RemoveTailInstructions();

            var endFinally = ilProcessor.EncapsulateWithTryFinally();

            ilProcessor.InsertBefore(endFinally, new[] {
                ilProcessor.Create(OpCodes.Ldloc, methodContextVariable),
                ilProcessor.Create(OpCodes.Callvirt, disposeMethodReference)
            }, true);

            ilProcessor.InsertBefore(ilProcessor.Body.Instructions[0], new[] {
                ilProcessor.Create(OpCodes.Ldstr, context.HitsPath),
                ilProcessor.Create(OpCodes.Ldstr, originalMethod.DeclaringType.Module.Assembly.Name.Name),
                ilProcessor.Create(OpCodes.Ldstr, originalMethod.DeclaringType.FullName),
                ilProcessor.Create(OpCodes.Ldstr, originalMethod.Name),
                ilProcessor.Create(OpCodes.Call, enterMethodReference),
                ilProcessor.Create(OpCodes.Stloc, methodContextVariable)
            }, true);

            if (instrumentInstructions && !methodDefinition.IsExcludedFromCodeCoverage())
            {
                InstrumentInstructions(
                    context,
                    methodDefinition,
                    instrumentedAssembly,
                    sequencePointsInstructions,
                    ilProcessor,
                    methodContextVariable,
                    instrumentedMethod);
            }

            ilProcessor.Body.OptimizeMacros();
        }

        private void InstrumentInstructions(
            InstrumentationContext context,
            MethodDefinition methodDefinition,
            InstrumentedAssembly instrumentedAssembly,
            IList<(SequencePoint sequencePoint, Instruction instruction)> sequencePointsInstructions,
            ILProcessor ilProcessor,
            VariableDefinition methodContextVariable,
            InstrumentedMethod instrumentedMethod)
        {
            var hitMethodReference = methodDefinition.Module.GetOrImportReference(hitMethodInfo);

            var excludedInstructions = new HashSet<Instruction>(GetExclusions(ilProcessor.Body.Instructions));

            var userSequencePointsInstructions = sequencePointsInstructions
                .Where(x => !x.sequencePoint.IsHidden && !excludedInstructions.Contains(x.instruction))
                .ToArray();

            var userInstructions = new HashSet<Instruction>(userSequencePointsInstructions
                .Select(x => x.instruction));

            var sequencePointByInstruction = userSequencePointsInstructions
                .ToDictionary(x => x.instruction, x => x.sequencePoint);

            var branchesBySequencePoint = methodDefinition.Body.Instructions[0]
                .ToGraph()
                .Filter(userInstructions)
                .GetBranches()
                .ToLookup(b => sequencePointByInstruction[b.Value]);

            var sequencePointsGroups = userSequencePointsInstructions
                .GroupBy(j => j.sequencePoint);

            foreach (var sequencePointGroup in sequencePointsGroups)
            {
                var sequencePoint = sequencePointGroup.Key;
                var instructions = sequencePointGroup.Select(x => x.instruction);

                var documentUrl = sequencePoint.Document.Url;

                var documentLines = _fileReader.ReadAllLines(new FileInfo(documentUrl));

                var code = documentLines.ExtractCode(
                    sequencePoint.StartLine,
                    sequencePoint.EndLine,
                    sequencePoint.StartColumn,
                    sequencePoint.EndColumn);

                if (code == null || code == "{" || code == "}")
                    continue;

                var firstInstruction = instructions.First();

                // if the previous instruction is a Prefix instruction then this instruction MUST go with it.
                // we cannot put an instruction between the two.
                if (firstInstruction.Previous != null && firstInstruction.Previous.OpCode.OpCodeType == OpCodeType.Prefix)
                    continue;

                if (!ilProcessor.Body.Instructions.Contains(firstInstruction))
                {
                    var methodFullName = $"{methodDefinition.DeclaringType.FullName}.{methodDefinition.Name}";
                    _logger.LogWarning("Skipping instruction because it was removed from method {method}", methodFullName);
                    continue;
                }

                var sourceRelativePath = GetSourceRelativePath(context, documentUrl);

                var instructionId = InstrumentInstruction(
                    firstInstruction,
                    ilProcessor,
                    methodContextVariable,
                    hitMethodReference,
                    context);

                var instrumentedConditions = new List<InstrumentedCondition>();

                foreach (var branch in branchesBySequencePoint[sequencePoint])
                {
                    var instrumentedBranches = new List<InstrumentedBranch>();

                    foreach (var child in branch.Children)
                    {
                        var branchId = InstrumentInstruction(
                            child.Value,
                            ilProcessor,
                            methodContextVariable,
                            hitMethodReference,
                            context
                        );

                        instrumentedBranches.Add(new InstrumentedBranch
                        {
                            HitId = branchId,
                            External = !instructions.Contains(child.Value),
                            Instruction = child.Value.ToString()
                        });
                    }

                    instrumentedConditions.Add(new InstrumentedCondition
                    {
                        Branches = instrumentedBranches.ToArray(),
                        Instruction = branch.Value.ToString()
                    });
                }

                instrumentedAssembly.AddSequence(sourceRelativePath, new InstrumentedSequence
                {
                    HitId = instructionId,
                    StartLine = sequencePoint.StartLine,
                    EndLine = sequencePoint.EndLine,
                    StartColumn = sequencePoint.StartColumn,
                    EndColumn = sequencePoint.EndColumn,
                    Instruction = firstInstruction.ToString(),
                    Method = instrumentedMethod,
                    Code = code,
                    Conditions = instrumentedConditions.ToArray()
                });
            }
        }

        private static int InstrumentInstruction(
            Instruction instruction,
            ILProcessor ilProcessor,
            VariableDefinition methodContextVariable,
            MethodReference hitMethodReference,
            InstrumentationContext context)
        {
            return instruction.GetOrAddExtension("Id", () =>
            {
                var id = ++context.UniqueId;
                ilProcessor.InsertBefore(instruction, new[]
                {
                    ilProcessor.Create(OpCodes.Ldloc, methodContextVariable),
                    ilProcessor.Create(OpCodes.Ldc_I4, id),
                    ilProcessor.Create(OpCodes.Callvirt, hitMethodReference)
                }, true);
                return id;
            });
        }

        private IEnumerable<Instruction> GetExclusions(IList<Instruction> instructions)
        {
            return LambdaInitPattern.FindInstructions(instructions);
        }

        private static string GetSourceRelativePath(InstrumentationContext context, string path)
        {
            var file = new Uri(path);
            var folder = new Uri(context.Workdir.FullName);
            string relativePath =
                Uri.UnescapeDataString(
                    folder.MakeRelativeUri(file)
                        .ToString()
                        .Replace('/', Path.DirectorySeparatorChar)
                );
            return relativePath;
        }
    }
}
