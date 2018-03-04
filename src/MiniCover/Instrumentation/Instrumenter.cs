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
using MiniCover.Utils;
using Newtonsoft.Json.Linq;

namespace MiniCover.Instrumentation
{
    public class Instrumenter
    {
        private int id;
        private readonly IList<string> assemblies;
        private readonly string hitsFile;
        private readonly IList<string> sourceFiles;
        private readonly string normalizedWorkDir;
        private readonly Type hitServiceType = typeof(HitService);
        private readonly IEnumerable<string> instrumentationDependencies;
        
        private readonly ConstructorInfo instrumentedAttributeConstructor = typeof(InstrumentedAttribute).GetConstructors().First();
        
        private InstrumentationResult result;

        public Instrumenter(IList<string> assemblies, string hitsFile, IList<string> sourceFiles, string workdir)
        {
            this.assemblies = assemblies;
            this.hitsFile = hitsFile;
            this.sourceFiles = sourceFiles;
            this.instrumentationDependencies = new[]
            {
                hitServiceType.Assembly.Location,
                typeof(JObject).Assembly.Location
            };
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

            foreach (var assemblyFile in assemblies)
            {
                RestoreBackup(assemblyFile);
            }

            var assemblyGroups = assemblies
                .Where(ShouldInstrumentAssembly)
                .GroupBy(FileUtils.GetFileHash)
                .ToArray();

            foreach (var assemblyGroup in assemblyGroups)
            {
                VisitAssemblyGroup(assemblyGroup);
            }

            return result;
        }

        private bool ShouldInstrumentAssembly(string assemblyFile)
        {
            if (IsBackupFile(assemblyFile))
                return false;

            if (!File.Exists(GetPdbFile(assemblyFile)))
                return false;

            return true;
        }

        private void VisitAssemblyGroup(IEnumerable<string> assemblyFiles)
        {
            var firstAssemblyFile = assemblyFiles.First();

            var instrumentedAssembly = InstrumentAssemblyIfNecessary(firstAssemblyFile);

            if (instrumentedAssembly == null)
                return;

            foreach (var assemblyFile in assemblyFiles)
            {
                var pdbFile = GetPdbFile(assemblyFile);
                var assemblyBackupFile = GetBackupFile(assemblyFile);
                var pdbBackupFile = GetBackupFile(pdbFile);

                //Backup
                File.Copy(assemblyFile, assemblyBackupFile, true);
                File.Copy(pdbFile, pdbBackupFile, true);

                //Override assembly
                File.Copy(instrumentedAssembly.TempAssemblyFile, assemblyFile, true);
                File.Copy(instrumentedAssembly.TempPdbFile, pdbFile, true);

                //Copy instrumentation dependencies
                var assemblyDirectory = Path.GetDirectoryName(assemblyFile);

                foreach (var dependencyPath in instrumentationDependencies)
                {
                    var dependencyAssemblyName = Path.GetFileName(dependencyPath);
                    var newDependencyPath = Path.Combine(assemblyDirectory, dependencyAssemblyName);
                    File.Copy(dependencyPath, newDependencyPath, true);
                    result.AddExtraAssembly(newDependencyPath);
                }
                
                instrumentedAssembly.AddLocation(
                    Path.GetFullPath(assemblyFile),
                    Path.GetFullPath(assemblyBackupFile),
                    Path.GetFullPath(pdbFile),
                    Path.GetFullPath(pdbBackupFile)
                );
            }

            result.AddInstrumentedAssembly(instrumentedAssembly);

            File.Delete(instrumentedAssembly.TempAssemblyFile);
            File.Delete(instrumentedAssembly.TempPdbFile);
        }

        private InstrumentedAssembly InstrumentAssemblyIfNecessary(string assemblyFile)
        {
            var assemblyDirectory = Path.GetDirectoryName(assemblyFile);

            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(assemblyDirectory);

            using (var assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyFile, new ReaderParameters { ReadSymbols = true, AssemblyResolver = resolver }))
            {
                if (!HasSourceFiles(assemblyDefinition))
                    return null;

                if (assemblyDefinition.CustomAttributes.Any(a => a.AttributeType.Name == "InstrumentedAttribute"))
                    throw new Exception($"Assembly \"{assemblyFile}\" is already instrumented");

                Console.WriteLine($"Instrumenting assembly \"{assemblyDefinition.Name.Name}\"");

                var instrumentedAssembly = new InstrumentedAssembly(assemblyDefinition.Name.Name);

                var instrumentedAttributeReference = assemblyDefinition.MainModule.ImportReference(instrumentedAttributeConstructor);
                assemblyDefinition.CustomAttributes.Add(new CustomAttribute(instrumentedAttributeReference));

                CreateAssemblyInit(assemblyDefinition);

                var hitMethodInfo = hitServiceType.GetMethod("Hit");
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
                    if (!sourceFiles.Contains(documentGroup.Key.Url))
                        continue;

                    var sourceRelativePath = GetSourceRelativePath(documentGroup.Key.Url);

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
                            var code = sequencePoint.ExtractCode(fileLines);
                            if (code == null || code == "{" || code == "}")
                                continue;

                            var instruction = instructions[sequencePoint.Offset];

                            // if the previous instruction is a Prefix instruction then this instruction MUST go with it.
                            // we cannot put an instruction between the two.
                            if (instruction.Previous != null && instruction.Previous.OpCode.OpCodeType == OpCodeType.Prefix)
                                continue;

                            var instructionId = ++id;

                            instrumentedAssembly.AddInstruction(sourceRelativePath, new InstrumentedInstruction
                            {
                                Id = instructionId,
                                StartLine = sequencePoint.StartLine,
                                EndLine = sequencePoint.EndLine,
                                StartColumn = sequencePoint.StartColumn,
                                EndColumn = sequencePoint.EndColumn,
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

                var miniCoverTempPath = GetMiniCoverTempPath();

                var instrumentedAssemblyFile = Path.Combine(miniCoverTempPath, $"{Guid.NewGuid()}.dll");
                var instrumentedPdbFile = GetPdbFile(instrumentedAssemblyFile);

                assemblyDefinition.Write(instrumentedAssemblyFile, new WriterParameters { WriteSymbols = true });

                instrumentedAssembly.TempAssemblyFile = instrumentedAssemblyFile;
                instrumentedAssembly.TempPdbFile = instrumentedPdbFile;

                return instrumentedAssembly;
            }
        }

        private bool HasSourceFiles(AssemblyDefinition assemblyDefinition)
        {
            return assemblyDefinition
                .GetAllMethods()
                .SelectMany(m => m.DebugInformation.SequencePoints)
                .Select(s => s.Document.Url)
                .Distinct()
                .Any(d => sourceFiles.Contains(d));
        }

        private void RestoreBackup(string assemblyFile)
        {
            var pdbFile = GetPdbFile(assemblyFile);
            var assemblyBackupFile = GetBackupFile(assemblyFile);
            var pdbBackupFile = GetBackupFile(pdbFile);

            if (File.Exists(assemblyBackupFile))
            {
                File.Copy(assemblyBackupFile, assemblyFile, true);
                File.Delete(assemblyBackupFile);
            }

            if (File.Exists(pdbBackupFile))
            {
                File.Copy(pdbBackupFile, pdbFile, true);
                File.Delete(pdbBackupFile);
            }
        }

        private void CreateAssemblyInit(AssemblyDefinition assemblyDefinition)
        {
            var initMethodInfo = this.hitServiceType.GetMethod("Init");
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
            Uri file = new Uri(path);
            Uri folder = new Uri(normalizedWorkDir);
            string relativePath =
                Uri.UnescapeDataString(
                    folder.MakeRelativeUri(file)
                        .ToString()
                        .Replace('/', Path.DirectorySeparatorChar)
                );
            return relativePath;
        }

        private string GetPdbFile(string assemblyFile)
        {
            return Path.ChangeExtension(assemblyFile, "pdb");
        }

        private string GetBackupFile(string file)
        {
            return Path.ChangeExtension(file, $"uninstrumented{Path.GetExtension(file)}");
        }

        private bool IsBackupFile(string file)
        {
            return Path.GetFileName(file).Contains(".uninstrumented");
        }

        private string GetMiniCoverTempPath()
        {
            var path = Path.Combine(Path.GetTempPath(), $"minicover");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}
