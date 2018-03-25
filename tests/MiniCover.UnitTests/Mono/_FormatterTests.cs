using Shouldly;
using Xunit;

namespace Mono.Cecil.Tests
{
    public class FormatterTests : BaseTestFixture
    {
        [Fact]
        public void ValidFormatShould_For_Exception_handler_type_try_finally()
        {
            var expectedIl = @".locals init ()
IL_0000: ldarg.0 // this
IL_0001: call System.Void System.Object::.ctor()
IL_0006: nop
IL_0007: nop
.try
{
IL_0008: nop
IL_0009: ldarg.0 // this
IL_000a: ldc.i4.5
IL_000b: stfld System.Int32 Sample.TryFinally.AClassWithATryFinallyInConstructor::value
IL_0010: nop
IL_0011: leave.s IL_001d
}
finally
{
IL_0013: nop
IL_0014: ldarg.0 // this
IL_0015: call System.Void Sample.TryFinally.AClassWithATryFinallyInConstructor::Exit()
IL_001a: nop
IL_001b: nop
IL_001c: endfinally
}
IL_001d: ret";

            TestModule("Sample.dll", module =>
            {
                var type = module.GetType("Sample.TryFinally.AClassWithATryFinallyInConstructor");
                var constructor = type.GetMethod(".ctor");
                Assert.NotNull(constructor);

                Normalize(Formatter.FormatMethodBody(constructor)).ShouldBe(Normalize(expectedIl));
            });
        }
    }
}