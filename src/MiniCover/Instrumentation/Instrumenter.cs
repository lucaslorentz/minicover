using MiniCover.Extensions;
using MiniCover.Model;
using MiniCover.Utils;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MiniCover.HitServices;
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
        private readonly Type methodContextType = typeof(HitService.MethodContext);
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
                hitServiceType.Assembly.Location
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

                foreach (var depsJsonFile in Directory.GetFiles(assemblyDirectory, "*.deps.json"))
                {
                    DepsJsonUtils.PatchDepsJson(depsJsonFile);
                }
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

                var enterMethodInfo = hitServiceType.GetMethod("EnterMethod");
                var exitMethodInfo = methodContextType.GetMethod("Exit");
                var hitInstructionMethodInfo = methodContextType.GetMethod("HitInstruction");

                var methodContextClassReference = assemblyDefinition.MainModule.ImportReference(methodContextType);
                var enterMethodReference = assemblyDefinition.MainModule.ImportReference(enterMethodInfo);
                var exitMethodReference = assemblyDefinition.MainModule.ImportReference(exitMethodInfo);

                var hitInstructionReference = assemblyDefinition.MainModule.ImportReference(hitInstructionMethodInfo);

                var methods = assemblyDefinition.GetAllMethods();

                var documentsGroups = methods
                    .SelectMany(m => m.DebugInformation.SequencePoints, (method, s) => new
                    {
                        Method = method,
                        SequencePoint = s,
                        s.Document
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
                        var methodDefinition = methodGroup.Key;

                        InstrumentMethod(assemblyDefinition, methodDefinition, methodGroup, methodContextClassReference, enterMethodReference, exitMethodReference, fileLines, instrumentedAssembly, sourceRelativePath, hitInstructionReference);
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

	    private Instruction GetFirstConstructorInstruction(MethodDefinition method)
	    {
		    var constructorInstruction = method.Body.Instructions.First(a => a.OpCode == OpCodes.Call);
		    return constructorInstruction;
	    }

        private void InstrumentMethod(AssemblyDefinition assemblyDefinition, MethodDefinition methodDefinition,
            IEnumerable<SequencePoint> sequencePoints,
            TypeReference methodContextClassReference,
            MethodReference enterMethodReference, MethodReference exitMethodReference,
            string[] fileLines, InstrumentedAssembly instrumentedAssembly, string sourceRelativePath,
            MethodReference hitInstructionReference)
        {
	        var ilProcessor = methodDefinition.Body.GetILProcessor();
	        ilProcessor.Body.InitLocals = true;
	        ilProcessor.Body.SimplifyMacros();

	        var instructions = methodDefinition.Body.Instructions.ToDictionary(i => i.Offset);

	        var methodContextVariable = new VariableDefinition(methodContextClassReference);
	        methodDefinition.Body.Variables.Add(methodContextVariable);
	        var pathParamLoadInstruction = ilProcessor.Create(OpCodes.Ldstr, hitsFile);
	        var enterMethodInstruction = ilProcessor.Create(OpCodes.Call, enterMethodReference);
	        var storeMethodResultInstruction = ilProcessor.Create(OpCodes.Stloc, methodContextVariable);
            foreach (var instruction in ilProcessor.Body.Instructions.ToArray())
            {
                if (instruction.OpCode == OpCodes.Tail)
                {
                    var noOpInstruction = ilProcessor.Create(OpCodes.Nop);
                    ilProcessor.Replace(instruction, noOpInstruction);
                    ReplaceInstructionReferences(methodDefinition, instruction, noOpInstruction);
                }
            }

	        var firstInstruction = instructions[0];

			
	        var loadMethodContextInstruction = ilProcessor.Create(OpCodes.Ldloc, methodContextVariable);
	        var exitMethodInstruction = ilProcessor.Create(OpCodes.Callvirt, exitMethodReference);
	        EncapsulateMethodBodyWithTryFinallyBlock(ilProcessor, assemblyDefinition.MainModule, methodDefinition, firstInstruction,
		        (processor, instruction) =>
		        {
			        ilProcessor.InsertBefore(instruction, exitMethodInstruction);
			        ilProcessor.InsertBefore(exitMethodInstruction, loadMethodContextInstruction);
		        });

            var currentFirstInstruction = methodDefinition.Body.Instructions.First();
	        ilProcessor.InsertBefore(currentFirstInstruction, storeMethodResultInstruction);
	        ilProcessor.InsertBefore(storeMethodResultInstruction, enterMethodInstruction);
	        ilProcessor.InsertBefore(enterMethodInstruction, pathParamLoadInstruction);
	        ReplaceInstructionReferences(methodDefinition, currentFirstInstruction, pathParamLoadInstruction);
			
            InstrumentInstructions(methodDefinition, sequencePoints, fileLines, instrumentedAssembly, sourceRelativePath, hitInstructionReference, instructions, ilProcessor, methodContextVariable);
            
            ilProcessor.Body.OptimizeMacros();
        }

		private void EncapsulateWithTryCatch(ILProcessor ilProcessor, MethodDefinition methodDefinition,
			Instruction from, Instruction to)
		{
			var tryEnd = Instruction.Create(OpCodes.Leave_S, to);
			ilProcessor.InsertBefore(to, tryEnd);

			Instruction tryStart = Instruction.Create(OpCodes.Nop);
			ilProcessor.InsertBefore(from, tryStart);
			
			Instruction rethrowInstruction = Instruction.Create(OpCodes.Rethrow);
			ilProcessor.InsertAfter(tryEnd, rethrowInstruction);
			var handler = new ExceptionHandler(ExceptionHandlerType.Catch)
			{
				TryStart = tryStart,
				TryEnd = rethrowInstruction,
				HandlerStart = rethrowInstruction,
				HandlerEnd = to,
				CatchType = methodDefinition.Module.ImportReference(typeof(Exception))
			};
			methodDefinition.Body.ExceptionHandlers.Add(handler);
		}
		private void EncapsulateMethodBodyWithTryFinallyBlock(ILProcessor ilProcessor,
			ModuleDefinition moduleDefinition, MethodDefinition methodDefinition, Instruction firstInstruction, Action<ILProcessor, Instruction> insertBeforReturn)
		{

			if (methodDefinition.IsConstructor)
			{
				var ctor = GetFirstConstructorInstruction(methodDefinition);
				if (methodDefinition.Body.Instructions.IndexOf(ctor) > 2)
				{
					var lastInstruction = methodDefinition.Body.Instructions.Last();
					EncapsulateWithTryCatch(ilProcessor, methodDefinition, firstInstruction, ctor.Previous);
					if (ctor.Next.Equals(lastInstruction)) return;
				}

			    if(firstInstruction.Next.OpCode != OpCodes.Nop)
			    {
			        firstInstruction = Instruction.Create(OpCodes.Nop);
			        ilProcessor.InsertAfter(ctor, firstInstruction);
			    }
			}
            
		    var returnStart = FixReturns(moduleDefinition, methodDefinition, ilProcessor);

			var beforeReturn = Instruction.Create(OpCodes.Endfinally);
			ilProcessor.InsertBefore(returnStart, beforeReturn);

			Instruction finallyStart = Instruction.Create(OpCodes.Nop);
			ilProcessor.InsertBefore(beforeReturn, finallyStart);
			insertBeforReturn(ilProcessor, beforeReturn);

			var handler = new ExceptionHandler(ExceptionHandlerType.Finally)
			{
				TryStart = firstInstruction,
				TryEnd = finallyStart,
				HandlerStart = finallyStart,
				HandlerEnd = returnStart,
			};

			methodDefinition.Body.ExceptionHandlers.Add(handler);
		}

        private Instruction FixReturns(ModuleDefinition moduleDefinition,
            MethodDefinition methodDefinition,
            ILProcessor ilProcessor)
        {
            if (methodDefinition.ReturnType == moduleDefinition.TypeSystem.Void)
            {
                var instructions = ilProcessor.Body.Instructions.ToArray();

                var newReturnInstruction = ilProcessor.Create(OpCodes.Ret);
                ilProcessor.Append(newReturnInstruction);

                foreach (var instruction in instructions)
                {
                    if (instruction.OpCode == OpCodes.Ret)
                    {
                        var leaveInstruction = ilProcessor.Create(OpCodes.Leave, newReturnInstruction);
                        ilProcessor.Replace(instruction, leaveInstruction);

                        ReplaceInstructionReferences(methodDefinition, instruction, leaveInstruction);
                    }
                }

                return newReturnInstruction;
            }
            else
            {
                var instructions = ilProcessor.Body.Instructions.ToArray();

                var returnVariable = new VariableDefinition(methodDefinition.ReturnType);
                ilProcessor.Body.Variables.Add(returnVariable);

                var loadResultInstruction = ilProcessor.Create(OpCodes.Ldloc, returnVariable);
                ilProcessor.Append(loadResultInstruction);
                var newReturnInstruction = ilProcessor.Create(OpCodes.Ret);
                ilProcessor.Append(newReturnInstruction);

                foreach (var instruction in instructions)
                {
                    if (instruction.OpCode == OpCodes.Ret)
                    {
                        var saveResultInstruction = ilProcessor.Create(OpCodes.Stloc, returnVariable);
                        ilProcessor.Replace(instruction, saveResultInstruction);
                        var leaveInstruction = ilProcessor.Create(OpCodes.Leave, loadResultInstruction);
                        ilProcessor.InsertAfter(saveResultInstruction, leaveInstruction);

                        ReplaceInstructionReferences(methodDefinition, instruction, leaveInstruction);
                    }
                }

                return loadResultInstruction;
            }
        }

        private void InstrumentInstructions(MethodDefinition methodDefinition, IEnumerable<SequencePoint> sequencePoints, string[] fileLines,
            InstrumentedAssembly instrumentedAssembly, string sourceRelativePath, MethodReference hitInstructionReference,
            Dictionary<int, Instruction> instructions, ILProcessor ilProcessor, VariableDefinition methodContextVariable)
        {
            foreach (var sequencePoint in sequencePoints)
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
                    Class = methodDefinition.DeclaringType.FullName,
                    Method = methodDefinition.Name,
                    MethodFullName = methodDefinition.FullName,
                    Instruction = instruction.ToString()
                });

                InstrumentInstruction(instructionId, instruction, hitInstructionReference, methodDefinition, ilProcessor,
                    methodContextVariable);
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

        private void InstrumentInstruction(int instructionId, Instruction instruction,
            MethodReference hitInstructionReference, MethodDefinition method, ILProcessor ilProcessor,
            VariableDefinition methodContextVariable)
        {
            var loadMethodContextInstruction = ilProcessor.Create(OpCodes.Ldloc, methodContextVariable);
            var lineParamLoadInstruction = ilProcessor.Create(OpCodes.Ldc_I4, instructionId);
            var registerInstruction = ilProcessor.Create(OpCodes.Callvirt, hitInstructionReference);

            ilProcessor.InsertBefore(instruction, registerInstruction);
            ilProcessor.InsertBefore(registerInstruction, lineParamLoadInstruction);
            ilProcessor.InsertBefore(lineParamLoadInstruction, loadMethodContextInstruction);

            ReplaceInstructionReferences(method, instruction, loadMethodContextInstruction);
        }

        private static void ReplaceInstructionReferences(MethodDefinition methodDefinition,
            Instruction oldInstruction,
            Instruction newInstruction)
        {
            //change try/finally etc to point to our first instruction if they referenced the one we inserted before
            foreach (var handler in methodDefinition.Body.ExceptionHandlers)
            {
                if (handler.FilterStart == oldInstruction)
                    handler.FilterStart = newInstruction;

                if (handler.TryStart == oldInstruction)
                    handler.TryStart = newInstruction;
                if (handler.TryEnd == oldInstruction)
                    handler.TryEnd = newInstruction;

                if (handler.HandlerStart == oldInstruction)
                    handler.HandlerStart = newInstruction;
                if (handler.HandlerEnd == oldInstruction)
                    handler.HandlerEnd = newInstruction;
            }

            //change instructions with a target instruction if they referenced the one we inserted before to be our first instruction
            foreach (var iteratedInstruction in methodDefinition.Body.Instructions)
            {
                var operand = iteratedInstruction.Operand;
                if (operand == oldInstruction)
                {
                    iteratedInstruction.Operand = newInstruction;
                    continue;
                }

                if (!(operand is Instruction[]))
                    continue;

                var operands = (Instruction[])operand;
                for (var i = 0; i < operands.Length; ++i)
                {
                    if (operands[i] == oldInstruction)
                        operands[i] = newInstruction;
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
            var path = Path.Combine(Path.GetTempPath(), "minicover");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}
