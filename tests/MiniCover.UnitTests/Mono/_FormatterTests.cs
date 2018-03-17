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
.try
{
IL_0006: ldarg.0 // this
IL_0007: ldc.i4.5
IL_0008: stfld System.Int32 AClassWithATryFinallyInConstructor::value
IL_000d: leave.s IL_0016
}
finally
{
IL_000f: ldarg.0 // this
IL_0010: call System.Void AClassWithATryFinallyInConstructor::Exit()
IL_0015: endfinally
}
IL_0016: ret";

            TestCSharp("Constructors.cs", module =>
            {
                var type = module.GetType("AClassWithATryFinallyInConstructor");
                var constructor = type.GetMethod(".ctor");
                Assert.NotNull(constructor);

                Normalize(Formatter.FormatMethodBody(constructor)).ShouldBe(Normalize(expectedIl));
            }, readOnly:true);
        }
    }
}