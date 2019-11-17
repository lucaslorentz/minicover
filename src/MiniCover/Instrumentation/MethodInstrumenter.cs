using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using MiniCover.Extensions;
using MiniCover.HitServices;
using MiniCover.Infrastructure.FileSystem;
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
        private static readonly MethodInfo hitInstructionMethodInfo = methodContextType.GetMethod("HitInstruction");

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
            var originalMethod = ResolveOriginalMethod(methodDefinition);

            var instrumentedMethod = instrumentedAssembly.AddMethod(new InstrumentedMethod
            {
                Class = originalMethod.DeclaringType.FullName,
                Name = originalMethod.Name,
                FullName = originalMethod.FullName,
            });

            var enterMethodInfo = hitServiceType.GetMethod("EnterMethod");
            var disposeMethodInfo = methodContextType.GetMethod("Dispose");

            var methodContextClassReference = methodDefinition.Module.GetOrImportReference(methodContextType);
            var enterMethodReference = methodDefinition.Module.GetOrImportReference(enterMethodInfo);
            var disposeMethodReference = methodDefinition.Module.GetOrImportReference(disposeMethodInfo);

            var ilProcessor = methodDefinition.Body.GetILProcessor();
            ilProcessor.Body.InitLocals = true;
            ilProcessor.Body.SimplifyMacros();

            var methodContextVariable = new VariableDefinition(methodContextClassReference);
            ilProcessor.Body.Variables.Add(methodContextVariable);

            ilProcessor.RemoveTailInstructions();

            var instructions = ilProcessor.Body.Instructions.ToDictionary(i => i.Offset);

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
                    instructions,
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
            Dictionary<int, Instruction> instructionsByOffset,
            ILProcessor ilProcessor,
            VariableDefinition methodContextVariable,
            InstrumentedMethod instrumentedMethod)
        {
            var hitInstructionReference = methodDefinition.Module.GetOrImportReference(hitInstructionMethodInfo);

            foreach (var sequencePoint in methodDefinition.DebugInformation.SequencePoints)
            {
                var document = sequencePoint.Document;

                if (document.FileHasChanged())
                {
                    _logger.LogInformation("Ignoring modified file {file}", document.Url);
                    continue;
                }

                if (sequencePoint.IsHidden)
                    continue;

                var documentUrl = sequencePoint.Document.Url;

                var documentLines = _fileReader.ReadAllLines(new FileInfo(documentUrl));

                var code = documentLines.ExtractCode(
                    sequencePoint.StartLine,
                    sequencePoint.EndLine,
                    sequencePoint.StartColumn,
                    sequencePoint.EndColumn);

                if (code == null || code == "{" || code == "}")
                    continue;

                var instruction = instructionsByOffset[sequencePoint.Offset];

                // if the previous instruction is a Prefix instruction then this instruction MUST go with it.
                // we cannot put an instruction between the two.
                if (instruction.Previous != null && instruction.Previous.OpCode.OpCodeType == OpCodeType.Prefix)
                    continue;

                if (!ilProcessor.Body.Instructions.Contains(instruction))
                {
                    var methodFullName = $"{methodDefinition.DeclaringType.FullName}.{methodDefinition.Name}";
                    _logger.LogWarning("Skipping instruction because it was removed from method {method}", methodFullName);
                    continue;
                }

                var sourceRelativePath = GetSourceRelativePath(context, documentUrl);

                var instructionId = ++context.InstructionId;

                instrumentedAssembly.AddInstruction(sourceRelativePath, new InstrumentedInstruction
                {
                    Id = instructionId,
                    StartLine = sequencePoint.StartLine,
                    EndLine = sequencePoint.EndLine,
                    StartColumn = sequencePoint.StartColumn,
                    EndColumn = sequencePoint.EndColumn,
                    Instruction = instruction.ToString(),
                    Method = instrumentedMethod,
                    Code = code
                });

                ilProcessor.InsertBefore(instruction, new[]
                {
                    ilProcessor.Create(OpCodes.Ldloc, methodContextVariable),
                    ilProcessor.Create(OpCodes.Ldc_I4, instructionId),
                    ilProcessor.Create(OpCodes.Callvirt, hitInstructionReference)
                }, true);
            }
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

        private MethodDefinition ResolveOriginalMethod(MethodDefinition methodDefinition)
        {
            var originalMethodName = ExtractOriginalMethodName(methodDefinition.Name)
                ?? ExtractOriginalMethodName(methodDefinition.DeclaringType.Name);

            if (!string.IsNullOrEmpty(originalMethodName)
                && methodDefinition.DeclaringType.IsCompilerGenerated())
            {
                var originalMethod = methodDefinition.DeclaringType.DeclaringType.Methods
                    .FirstOrDefault(m => m.Name == originalMethodName);

                if (originalMethod != null)
                    return originalMethod;
            }

            return methodDefinition;
        }

        private string ExtractOriginalMethodName(string name)
        {
            var lessThanIndex = name.IndexOf("<");
            var greaterThanIndex = name.IndexOf(">");

            if (lessThanIndex == -1 || greaterThanIndex == -1 || lessThanIndex + 1 == greaterThanIndex)
                return null;

            return name.Substring(lessThanIndex + 1, greaterThanIndex - lessThanIndex - 1);
        }
    }
}
