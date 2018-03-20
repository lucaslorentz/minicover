using System;
using System.Collections.Generic;
using System.Linq;
using MiniCover.HitServices;
using MiniCover.Model;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Pdb;
using Mono.Cecil.Rocks;
using Mono.Cecil.Tests;
using Shouldly;
using Xunit;

namespace MiniCover.UnitTests
{
    public class InstrumenterTests : BaseTestFixture
    {
        [Fact]
        public void InstrumentMethodShould_Not_EncapsulateCtor_with_existing_try_finally()
        {
            var expectedIl = @".locals init (MiniCover.HitServices.HitService/MethodContext V_0)
IL_0000: ldstr ""test-file""
IL_0005: call MiniCover.HitServices.HitService/MethodContext MiniCover.HitServices.HitService::EnterMethod(System.String)
IL_000a: stloc.0
IL_000b: ldarg.0 // this
IL_000c: call System.Void System.Object::.ctor()
.try
{
IL_0011: nop
IL_0012: nop
IL_0013: nop
.try
{
IL_0014: nop
IL_0015: ldarg.0 // this
IL_0016: ldc.i4.5
IL_0017: stfld System.Int32 Sample.TryFinally.AClassWithATryFinallyInConstructor::value
IL_001c: nop
IL_001d: leave.s IL_0029
}
finally
{
IL_001f: nop
IL_0020: ldarg.0 // this
IL_0021: call System.Void Sample.TryFinally.AClassWithATryFinallyInConstructor::Exit()
IL_0026: nop
IL_0027: nop
IL_0028: endfinally
}
IL_0029: leave.s IL_0033
}
finally
{
IL_002b: nop
IL_002c: ldloc.0
IL_002d: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Exit()
IL_0032: endfinally
}
IL_0033: ret";

            TestModule("Sample.dll", module =>
            {
                var type = module.GetType("Sample.TryFinally.AClassWithATryFinallyInConstructor");
                var constructor = type.GetMethod(".ctor");
                Assert.NotNull(constructor);
                var enterMethodInfo = typeof(HitService).GetMethod("EnterMethod");
                var exitMethodInfo = typeof(HitService.MethodContext).GetMethod("Exit");
                var hitInstructionMethodInfo = typeof(HitService.MethodContext).GetMethod("HitInstruction");

                var methodContextClassReference = module.ImportReference(typeof(HitService.MethodContext));
                var enterMethodReference = module.ImportReference(enterMethodInfo);
                var exitMethodReference = module.ImportReference(exitMethodInfo);

                var hitInstructionReference = module.ImportReference(hitInstructionMethodInfo);
                InstrumentMethod(module, constructor, Enumerable.Empty<SequencePoint>(), methodContextClassReference, enterMethodReference, exitMethodReference, new string[0], new InstrumentedAssembly(module.Assembly.Name.Name), "", hitInstructionReference, "test-file");
                Normalize(Formatter.FormatMethodBody(constructor)).ShouldBe(Normalize(expectedIl));
            });
        }


        private void InstrumentMethod(ModuleDefinition moduleDefinition, MethodDefinition methodDefinition,
             IEnumerable<SequencePoint> sequencePoints,
             TypeReference methodContextClassReference,
             MethodReference enterMethodReference, MethodReference exitMethodReference,
             string[] fileLines, InstrumentedAssembly instrumentedAssembly, string sourceRelativePath,
             MethodReference hitInstructionReference, string hitsFile)
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

            var firstInstruction = instructions[0];


            var loadMethodContextInstruction = ilProcessor.Create(OpCodes.Ldloc, methodContextVariable);
            var exitMethodInstruction = ilProcessor.Create(OpCodes.Callvirt, exitMethodReference);
            ilProcessor.EncapsulateMethodBodyWithTryFinallyBlock(firstInstruction, (processor, instruction) =>
{
    ilProcessor.InsertBefore(instruction, exitMethodInstruction);
    ilProcessor.InsertBefore(exitMethodInstruction, loadMethodContextInstruction);
});
            ilProcessor.InsertBefore(firstInstruction, storeMethodResultInstruction);
            ilProcessor.InsertBefore(storeMethodResultInstruction, enterMethodInstruction);
            ilProcessor.InsertBefore(enterMethodInstruction, pathParamLoadInstruction);
            ilProcessor.ReplaceInstructionReferences(firstInstruction, pathParamLoadInstruction);

            //InstrumentInstructions(methodDefinition, sequencePoints, fileLines, instrumentedAssembly, sourceRelativePath, hitInstructionReference, instructions, ilProcessor, methodContextVariable);
            foreach (var instruction in ilProcessor.Body.Instructions.ToArray())
            {
                if (instruction.OpCode == OpCodes.Tail)
                {
                    var noOpInstruction = ilProcessor.Create(OpCodes.Nop);
                    ilProcessor.Replace(instruction, noOpInstruction);
                    ilProcessor.ReplaceInstructionReferences(instruction, noOpInstruction);
                }
            }
            ilProcessor.Body.OptimizeMacros();
        }


    }
}