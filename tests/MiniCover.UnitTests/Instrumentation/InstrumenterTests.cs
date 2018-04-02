using System.Collections.Generic;
using System.Linq;
using MiniCover.HitServices;
using MiniCover.Model;
using Mono.Cecil;
using Mono.Cecil.Cil;
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


        [Fact]
        public void InstrumentMethodShould_Not_Encapsulate_HitService_EnterMethod_in_a_try_catch()
        {
            var expectedIl = @".locals init (MiniCover.HitServices.HitService/MethodContext V_0)
IL_0000: ldstr ""test-file""
IL_0005: call MiniCover.HitServices.HitService/MethodContext MiniCover.HitServices.HitService::EnterMethod(System.String)
IL_000a: stloc.0
IL_000b: nop
.try
{
IL_000c: nop
IL_000d: ldarg.0 // this
IL_000e: newobj System.Void Sample.AnotherClass::.ctor()
IL_0013: stfld Sample.AnotherClass Sample.TryFinally.HeritingClass::<Instance>k__BackingField
IL_0018: leave.s IL_001c
}
catch System.Exception
{
IL_001a: rethrow
}
IL_001c: ldarg.0 // this
IL_001d: ldarg.1
IL_001e: call System.Void Sample.TryFinally.AnAbstractClass::.ctor(System.Int32)
.try
{
IL_0023: nop
IL_0024: nop
IL_0025: nop
IL_0026: ldarg.0 // this
IL_0027: ldc.i4.s 15
IL_0029: ldarg.1
IL_002a: mul
IL_002b: stfld System.Int32 Sample.TryFinally.HeritingClass::<Value>k__BackingField
IL_0030: leave.s IL_003a
}
finally
{
IL_0032: nop
IL_0033: ldloc.0
IL_0034: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Exit()
IL_0039: endfinally
}
IL_003a: ret";

            TestModule("Sample.dll", module =>
            {
                var type = module.GetType("Sample.TryFinally.HeritingClass");
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

        [Fact]
        public void InstrumentMethodShould_Not_Encapsulate_HitService_EnterMethod_in_a_try_catch2()
        {
            var expectedIl = @".locals init (MiniCover.HitServices.HitService/MethodContext V_0)
IL_0000: ldstr ""test-file""
IL_0005: call MiniCover.HitServices.HitService/MethodContext MiniCover.HitServices.HitService::EnterMethod(System.String)
IL_000a: stloc.0
IL_000b: ldarg.0 // this
IL_000c: ldarg.1
IL_000d: ldarg.2
IL_000e: ldarg.3
IL_000f: ldarg.s 4
IL_0011: ldarg.s 5
IL_0013: ldarg.s 6
IL_0015: newobj System.Void System.Collections.Generic.List`1<System.Object>::.ctor()
IL_001a: call System.Void zipkin4net.SpanState::.ctor(System.Int64,System.Int64,System.Nullable`1<System.Int64>,System.Int64,System.Nullable`1<System.Boolean>,System.Boolean,System.Collections.Generic.List`1<System.Object>)
IL_001f: nop
IL_0020: nop
IL_0021: ldloc.0
IL_0022: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Exit()
IL_0027: ret";

            TestModule("zipkin4net.dll", module =>
            {
                var type = module.GetType("zipkin4net.SpanState");
                var constructor = type.GetConstructors().FirstOrDefault(a => a.HasParameters && a.Parameters.Count == 6);
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

        [Fact]
        public void InstrumentMethodShould_Not_Encapsulate_HitService_EnterMethod_in_a_try_generic()
        {
            var expectedIl = @".locals init (MiniCover.HitServices.HitService/MethodContext V_0)
IL_0000: ldstr ""test-file""
IL_0005: call MiniCover.HitServices.HitService/MethodContext MiniCover.HitServices.HitService::EnterMethod(System.String)
IL_000a: stloc.0
IL_000b: ldarg.0 // this
IL_000c: ldarg.1
IL_000d: ldarg.2
IL_000e: ldarg.3
IL_000f: call System.Collections.Generic.IDictionary`2<System.String,K> zipkin4net.Propagation.ExtraFieldPropagation`1<K>::CreateNameToKey(System.Collections.Generic.IEnumerable`1<System.String>,zipkin4net.Propagation.KeyFactory`1<K>)
IL_0014: call System.Void zipkin4net.Propagation.ExtraFieldPropagation`1<K>::.ctor(zipkin4net.Propagation.IPropagation`1<K>,System.Collections.Generic.IDictionary`2<System.String,K>)
IL_0019: nop
IL_001a: nop
IL_001b: ldloc.0
IL_001c: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Exit()
IL_0021: ret";

            TestModule("zipkin4net.dll", module =>
            {
                var type = module.GetType("zipkin4net.Propagation.ExtraFieldPropagation`1");
                var constructor = type.GetConstructors().FirstOrDefault();
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

        [Fact]
        public void InstrumentMethodShould_Not_Encapsulate_HitService_EnterMethod_in_a_try_catch_Static()
        {
            var expectedIl = @".locals init (MiniCover.HitServices.HitService/MethodContext V_0)
IL_0000: ldstr ""test-file""
IL_0005: call MiniCover.HitServices.HitService/MethodContext MiniCover.HitServices.HitService::EnterMethod(System.String)
IL_000a: stloc.0
IL_000b: nop
.try
{
IL_000c: newobj System.Void System.Threading.AsyncLocal`1<zipkin4net.Trace>::.ctor()
IL_0011: stsfld System.Threading.AsyncLocal`1<zipkin4net.Trace> zipkin4net.TraceContext::AsyncLocalTrace
IL_0016: ldstr ""Mono.Runtime""
IL_001b: call System.Type System.Type::GetType(System.String)
IL_0020: ldnull
IL_0021: cgt.un
IL_0023: stsfld System.Boolean zipkin4net.TraceContext::IsRunningOnMono
IL_0028: leave.s IL_0032
}
finally
{
IL_002a: nop
IL_002b: ldloc.0
IL_002c: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Exit()
IL_0031: endfinally
}
IL_0032: ret";

            TestModule("zipkin4net.dll", module =>
            {
                var type = module.GetType("zipkin4net.TraceContext");
                var constructor = type.GetConstructors().FirstOrDefault();
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
            var currentFirstInstruction = ilProcessor.Body.Instructions.First();
            ilProcessor.InsertBefore(currentFirstInstruction, storeMethodResultInstruction);
            ilProcessor.InsertBefore(storeMethodResultInstruction, enterMethodInstruction);
            ilProcessor.InsertBefore(enterMethodInstruction, pathParamLoadInstruction);
            ilProcessor.ReplaceInstructionReferences(currentFirstInstruction, pathParamLoadInstruction);

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