using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using MiniCover.Extensions;
using MiniCover.HitServices;
using MiniCover.Infrastructure.FileSystem;
using MiniCover.Instrumentation.Branches;
using MiniCover.Instrumentation.Patterns;
using MiniCover.Model;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace MiniCover.Instrumentation
{
    public class MethodInstrumenter
    {
        private static readonly Type hitServiceType = typeof(HitService);
        private static readonly Type methodContextType = typeof(HitService.MethodContext);
        private static readonly MethodInfo hitMethodInfo = methodContextType.GetMethod(nameof(HitService.MethodContext.Hit));
        private static readonly MethodInfo enterMethodInfo = hitServiceType.GetMethod(nameof(HitService.EnterMethod));
        private static readonly MethodInfo disposeMethodInfo = methodContextType.GetMethod(nameof(IDisposable.Dispose));

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

            var instrumentedMethod = instrumentedAssembly.AddMethod(new InstrumentedMethod
            {
                Class = originalMethod.DeclaringType.FullName,
                Name = originalMethod.Name,
                FullName = originalMethod.FullName,
            });

            var methodContextClassReference = methodDefinition.Module.GetOrImportReference(methodContextType);
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

            var excludedInstructions = GetExclusions(ilProcessor.Body.Instructions)
                .Distinct()
                .ToHashSet();

            var userSequencePointsInstructions = sequencePointsInstructions
                .Where(x => !x.sequencePoint.IsHidden && !excludedInstructions.Contains(x.instruction))
                .ToArray();

            var userInstructions = userSequencePointsInstructions
                .Select(x => x.instruction)
                .ToHashSet();

            var sequencePointByInstruction = userSequencePointsInstructions
                .ToDictionary(x => x.instruction, x => x.sequencePoint);

            var branchesBySequencePoint = BranchCollector.Collect(methodDefinition.Body.Instructions[0], userInstructions)
                .ToLookup(b => sequencePointByInstruction[b.PivotInstruction]);

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

                    foreach (var targetInstruction in branch.Targets)
                    {
                        var branchId = InstrumentInstruction(
                            targetInstruction,
                            ilProcessor,
                            methodContextVariable,
                            hitMethodReference,
                            context
                        );

                        instrumentedBranches.Add(new InstrumentedBranch
                        {
                            HitId = branchId,
                            External = !instructions.Contains(targetInstruction),
                            Instruction = targetInstruction.ToString()
                        });
                    }

                    instrumentedConditions.Add(new InstrumentedCondition
                    {
                        Branches = instrumentedBranches.ToArray(),
                        Instruction = branch.PivotInstruction.ToString()
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
