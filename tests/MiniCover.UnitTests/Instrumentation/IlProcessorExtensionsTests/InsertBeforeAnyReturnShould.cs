using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Pdb;
using Mono.Cecil.Rocks;
using Mono.Cecil.Tests;
using Shouldly;
using Xunit;

namespace MiniCover.UnitTests.Instrumentation.IlProcessorExtensionsTests
{
    public class InsertBeforeAnyReturnShould : BaseTestFixture
    {
        [Fact]
        public void InjectANopBeforeTheOnlyReturn()
        {
            var expectedIl = @".locals init (System.Int32 V_0)

// [6 6 - 9 10]
IL_0000: nop

// [7 7 - 13 30]
IL_0001: ldarg.1
IL_0002: ldc.i4.2
IL_0003: mul
IL_0004: stloc.0
IL_0005: br.s IL_0007

// [8 8 - 9 10]
IL_0007: ldloc.0
IL_0008: nop
IL_0009: ret";

            TestModule("Sample.dll", module =>
            {
                var type = module.GetType("Sample.TryFinally.AnotherClassWithoutTryFinally");
                var method = type.GetMethod("MultiplyByTwo");
                Assert.NotNull(method);
                ApplyInstrumentation(method);
                Normalize(Formatter.FormatMethodBody(method)).ShouldBe(Normalize(expectedIl));
            }, typeof(PdbReaderProvider));
        }

        [Fact]
        public void InjectANopBefore2TheOnlyReturn()
        {
            var expectedIl = @".locals init ()

// [8 8 - 9 52]
IL_0000: ldarg.0 // this
IL_0001: call System.Void System.Object::.ctor()
IL_0006: nop

// [9 9 - 9 10]
IL_0007: nop
.try
{

// [11 11 - 13 14]
IL_0008: nop

// [12 12 - 17 27]
IL_0009: ldarg.0 // this
IL_000a: ldc.i4.5
IL_000b: stfld System.Int32 Sample.TryFinally.AClassWithATryFinallyInConstructor::value

// [13 13 - 13 14]
IL_0010: nop
IL_0011: leave.s IL_001e
}
finally
{

// [15 15 - 13 14]
IL_0013: nop

// [16 16 - 17 24]
IL_0014: ldarg.0 // this
IL_0015: call System.Void Sample.TryFinally.AClassWithATryFinallyInConstructor::Exit()
IL_001a: nop

// [17 17 - 13 14]
IL_001b: nop
IL_001c: endfinally
}
IL_001d: nop

// [18 18 - 9 10]
IL_001e: ret";

            TestModule("Sample.dll", module =>
            {
                var type = module.GetType("Sample.TryFinally.AClassWithATryFinallyInConstructor");
                var method = type.GetConstructors().First();
                Assert.NotNull(method);
                ApplyInstrumentation(method);
                Normalize(Formatter.FormatMethodBody(method)).ShouldBe(Normalize(expectedIl));
            }, typeof(PdbReaderProvider));
        }


        private void ApplyInstrumentation(MethodDefinition methodDefinition)
        {
            var processor = methodDefinition.Body.GetILProcessor();
            processor.InsertBeforeAnyReturn((ilProcessor, instruction) => { ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Nop)); });

            processor.Body.OptimizeMacros();
        }
    }
}