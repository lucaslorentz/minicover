using MiniCover.Extensions;
using MiniCover.Model;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MiniCover.Instrumentation
{
    public class Instrumenter
    {
        private int id = 0;
        private IList<string> assemblies;
        private string hitsFile;
        private IList<string> sourceFiles;
        private string normalizedWorkDir;

        private InstrumentationResult result;

        public Instrumenter(IList<string> assemblies, string hitsFile, IList<string> sourceFiles, string workdir)
        {
            this.assemblies = assemblies;
            this.hitsFile = hitsFile;
            this.sourceFiles = sourceFiles;

            normalizedWorkDir = workdir;
            if (!normalizedWorkDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                normalizedWorkDir += Path.DirectorySeparatorChar;
        }

        public InstrumentationResult Execute()
        {
            id = 0;

            result = new InstrumentationResult
            {
                SourcePath = normalizedWorkDir,
                HitsFile = hitsFile
            };

            foreach (var fileName in assemblies)
            {
                InstrumentAssembly(fileName);
            }

            foreach (var assembly in result.Assemblies)
            {
                assembly.Value.Files = assembly.Value.Files.OrderBy(kv => kv.Key).ToDictionary(kv => kv.Key, kv => kv.Value);
            }

            return result;
        }

        private void InstrumentAssembly(string assemblyFile)
        {
            var pdbFile = Path.ChangeExtension(assemblyFile, "pdb");
            if (!File.Exists(pdbFile))
                return;

            if (assemblyFile.EndsWith("uninstrumented.dll"))
                return;

            var assemblyBackupFile = Path.ChangeExtension(assemblyFile, "uninstrumented.dll");
            if (File.Exists(assemblyBackupFile))
                File.Copy(assemblyBackupFile, assemblyFile, true);

            var pdbBackupFile = Path.ChangeExtension(pdbFile, "uninstrumented.pdb");
            if (File.Exists(pdbBackupFile))
                File.Copy(pdbBackupFile, pdbFile, true);

            if (!HasSourceFiles(assemblyFile))
                return;

            if (IsInstrumented(assemblyFile))
                throw new Exception($"Assembly \"{assemblyFile}\" is already instrumented");

            Console.WriteLine($"Instrumenting assembly \"{assemblyFile}\"");

            File.Copy(assemblyFile, assemblyBackupFile, true);
            File.Copy(pdbFile, pdbBackupFile, true);

            var assemblyDirectory = Path.GetDirectoryName(assemblyFile);

            using (var assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyBackupFile, new ReaderParameters { ReadSymbols = true }))
            {
                var instrumentedAssembly = result.AddInstrumentedAssembly(
                    assemblyDefinition.Name.Name,
                    Path.GetFullPath(assemblyBackupFile),
                    Path.GetFullPath(assemblyFile),
                    Path.GetFullPath(pdbBackupFile),
                    Path.GetFullPath(pdbFile)
                );

                var instrumentedConstructor = typeof(InstrumentedAttribute).GetConstructors().First();
                var instrumentedReference = assemblyDefinition.MainModule.ImportReference(instrumentedConstructor);
                assemblyDefinition.CustomAttributes.Add(new CustomAttribute(instrumentedReference));

                var miniCoverAssemblyPath = typeof(HitService).GetTypeInfo().Assembly.Location;
                var miniCoverAssemblyName = Path.GetFileName(miniCoverAssemblyPath);
                var newMiniCoverAssemblyPath = Path.Combine(assemblyDirectory, miniCoverAssemblyName);
                File.Copy(miniCoverAssemblyPath, newMiniCoverAssemblyPath, true);
                result.AddExtraAssembly(newMiniCoverAssemblyPath);

                CreateAssemblyInit(assemblyDefinition);

                var hitMethodInfo = typeof(HitService).GetMethod("Hit");
                var hitMethodReference = assemblyDefinition.MainModule.ImportReference(hitMethodInfo);

                var methods = assemblyDefinition.GetAllMethods();

                var documentsGroups = methods
                    .SelectMany(m => m.DebugInformation.SequencePoints, (m, s) => new
                    {
                        Method = m,
                        SequencePoint = s,
                        Document = s.Document
                    })
                    .GroupBy(j => j.Document)
                    .ToArray();

                foreach (var documentGroup in documentsGroups)
                {
                    var sourceRelativePath = GetSourceRelativePath(documentGroup.Key.Url);
                    if (sourceRelativePath == null)
                        continue;

                    if (documentGroup.Key.FileHasChanged())
                    {
                        Console.WriteLine($"Ignoring modified file \"{documentGroup.Key.Url}\"");
                        continue;
                    }

                    var fileLines = File.ReadAllLines(documentGroup.Key.Url);

                    var methodGroups = documentGroup
                        .GroupBy(j => j.Method, j => j.SequencePoint)
                        .ToArray();

                    foreach (var methodGroup in methodGroups)
                    {
                        var ilProcessor = methodGroup.Key.Body.GetILProcessor();

                        ilProcessor.Body.SimplifyMacros();

                        var instructions = methodGroup.Key.Body.Instructions.ToDictionary(i => i.Offset);

                        foreach (var sequencePoint in methodGroup)
                        {
                            var code = ExtractCode(fileLines, sequencePoint);
                            if (code == null || code == "{" || code == "}")
                                continue;

                            var instruction = instructions[sequencePoint.Offset];

                            // if the previous instruction is a Prefix instruction then this instruction MUST go with it.
                            // we cannot put an instruction between the two.
                            if (instruction.Previous != null && instruction.Previous.OpCode.OpCodeType == OpCodeType.Prefix)
                                return;

                            var instructionId = ++id;

                            instrumentedAssembly.AddInstruction(sourceRelativePath, new InstrumentedInstruction
                            {
                                Id = instructionId,
                                StartLine = sequencePoint.StartLine,
                                EndLine = sequencePoint.EndLine,
                                StartColumn = sequencePoint.StartColumn,
                                EndColumn = sequencePoint.EndColumn,
                                Assembly = assemblyDefinition.Name.Name,
                                Class = methodGroup.Key.DeclaringType.FullName,
                                Method = methodGroup.Key.Name,
                                MethodFullName = methodGroup.Key.FullName,
                                Instruction = instruction.ToString()
                            });

                            InstrumentInstruction(instructionId, instruction, hitMethodReference, methodGroup.Key, ilProcessor);
                        }

                        ilProcessor.Body.OptimizeMacros();
                    }
                }

                assemblyDefinition.Write(assemblyFile, new WriterParameters { WriteSymbols = true });
            }
        }

        private void CreateAssemblyInit(AssemblyDefinition assemblyDefinition)
        {
            var initMethodInfo = typeof(HitService).GetMethod("Init");
            var initMethodReference = assemblyDefinition.MainModule.ImportReference(initMethodInfo);
            var moduleType = assemblyDefinition.MainModule.GetType("<Module>");
            var moduleConstructor = moduleType.FindOrCreateCctor();
            var ilProcessor = moduleConstructor.Body.GetILProcessor();

            var initInstruction = ilProcessor.Create(OpCodes.Call, initMethodReference);
            if (moduleConstructor.Body.Instructions.Count > 0)
                ilProcessor.InsertBefore(moduleConstructor.Body.Instructions[0], initInstruction);
            else
                ilProcessor.Append(initInstruction);

            var pathParamLoadInstruction = ilProcessor.Create(OpCodes.Ldstr, hitsFile);
            ilProcessor.InsertBefore(initInstruction, pathParamLoadInstruction);
        }

        private bool HasSourceFiles(string assemblyFile)
        {
            using (var assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyFile, new ReaderParameters { ReadSymbols = true }))
            {
                var methods = assemblyDefinition.GetAllMethods();

                return methods
                    .SelectMany(m => m.DebugInformation.SequencePoints)
                    .Select(s => s.Document.Url)
                    .Distinct()
                    .Any(d => GetSourceRelativePath(d) != null);
            }
        }

        private bool IsInstrumented(string assemblyFile)
        {
            using (var assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyFile))
            {
                return assemblyDefinition.CustomAttributes.Any(a => a.AttributeType.Name == "InstrumentedAttribute");
            }
        }

        private void InstrumentInstruction(int instructionId, Instruction instruction,
            MethodReference hitMethodReference, MethodDefinition method, ILProcessor ilProcessor)
        {
            var pathParamLoadInstruction = ilProcessor.Create(OpCodes.Ldstr, hitsFile);
            var lineParamLoadInstruction = ilProcessor.Create(OpCodes.Ldc_I4, instructionId);
            var registerInstruction = ilProcessor.Create(OpCodes.Call, hitMethodReference);

            ilProcessor.InsertBefore(instruction, registerInstruction);
            ilProcessor.InsertBefore(registerInstruction, lineParamLoadInstruction);
            ilProcessor.InsertBefore(lineParamLoadInstruction, pathParamLoadInstruction);

            var newFirstInstruction = pathParamLoadInstruction;

            //change try/finally etc to point to our first instruction if they referenced the one we inserted before
            foreach (var handler in method.Body.ExceptionHandlers)
            {
                if (handler.FilterStart == instruction)
                    handler.FilterStart = newFirstInstruction;

                if (handler.TryStart == instruction)
                    handler.TryStart = newFirstInstruction;
                if (handler.TryEnd == instruction)
                    handler.TryEnd = newFirstInstruction;

                if (handler.HandlerStart == instruction)
                    handler.HandlerStart = newFirstInstruction;
                if (handler.HandlerEnd == instruction)
                    handler.HandlerEnd = newFirstInstruction;
            }

            //change instructions with a target instruction if they referenced the one we inserted before to be our first instruction
            foreach (var iteratedInstruction in method.Body.Instructions)
            {
                var operand = iteratedInstruction.Operand;
                if (operand == instruction)
                {
                    iteratedInstruction.Operand = newFirstInstruction;
                    continue;
                }

                if (!(operand is Instruction[]))
                    continue;

                var operands = (Instruction[])operand;
                for (var i = 0; i < operands.Length; ++i)
                {
                    if (operands[i] == instruction)
                        operands[i] = newFirstInstruction;
                }
            }
        }

        private string GetSourceRelativePath(string path)
        {
            if (!path.StartsWith(normalizedWorkDir))
                return null;

            if (!sourceFiles.Contains(path))
                return null;

            return path.Substring(normalizedWorkDir.Length);
        }

        private string ExtractCode(string[] fileLines, SequencePoint sequencePoint)
        {
            if (sequencePoint.IsHidden)
                return null;

            if (sequencePoint.StartLine == sequencePoint.EndLine)
            {
                var lineIndex = sequencePoint.StartLine - 1;
                if (lineIndex < 0 || lineIndex >= fileLines.Length)
                    return null;

                var startIndex = sequencePoint.StartColumn - 1;
                if (startIndex < 0 || startIndex >= fileLines[lineIndex].Length)
                    return null;

                var length = sequencePoint.EndColumn - sequencePoint.StartColumn;
                if (length <= 0 || startIndex + length > fileLines[lineIndex].Length)
                    return null;

                return fileLines[lineIndex].Substring(startIndex, length);
            }
            else
            {
                var result = new List<string>();

                var firstLineIndex = sequencePoint.StartLine - 1;
                if (firstLineIndex < 0 || firstLineIndex >= fileLines.Length)
                    return null;

                var startColumnIndex = sequencePoint.StartColumn - 1;
                if (startColumnIndex < 0 || startColumnIndex >= fileLines[firstLineIndex].Length)
                    return null;

                var lastLineIndex = sequencePoint.EndLine - 1;
                if (lastLineIndex < firstLineIndex || lastLineIndex >= fileLines.Length)
                    return null;

                var endLineLength = sequencePoint.EndColumn - 1;
                if (endLineLength <= 0 || endLineLength > fileLines[lastLineIndex].Length)
                    return null;

                result.Add(fileLines[firstLineIndex].Substring(startColumnIndex));

                for (var l = firstLineIndex + 1; l < lastLineIndex; l++)
                    result.Add(fileLines[l]);

                result.Add(fileLines[lastLineIndex].Substring(0, endLineLength));

                return string.Join(Environment.NewLine, result);
            }
        }
    }
}