using Mono.Cecil.Tests;
using Shouldly;
using Xunit;

namespace MiniCover.UnitTests
{
    public class InstrumenterTests:BaseTestFixture
    {
		[Fact]
	    public void EncapsulateWithTryFinallyShould_Not_EncapsulateCtor_with_a_field_initialized_out_side_the_constructor()
		{
			var expectedIl = @".locals ()
								IL_0000: ldarg.0
								IL_0001: ldc.i4.5
								IL_0002: stfld System.Int32 AClassWithFieldInitializedOutsideConstructor::value
								IL_0007: ldarg.0
								IL_0008: call System.Void System.Object::.ctor()
								IL_000d: ret";

			TestCSharp("Constructors.cs", module =>
			{
				var type = module.GetType("AClassWithFieldInitializedOutsideConstructor");
				var constructor = type.GetMethod(".ctor");
				Assert.NotNull(constructor);

				Normalize(Formatter.FormatMethodBody(constructor)).ShouldBe(Normalize(expectedIl));
			});
		}

        [Fact]
        public void EncapsulateWithTryFinallyShould_Not_EncapsulateCtor_with_existing_try_finally_from_dll()
        {
            var expectedIl = @".locals init ()
IL_0000: ldarg.0
IL_0001: call System.Void System.Object::.ctor()
IL_0006: nop
IL_0007: nop
IL_0008: nop
.try
{
IL_0009: ldarg.0
IL_000a: ldc.i4.5
IL_000b: stfld System.Int32 Sample.TryFinally.AClassWithATryFinallyInConstructor::value
IL_0010: nop
IL_0011: leave.s IL_001d
}
finally
{
IL_0013: nop
IL_0014: ldarg.0
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

	    [Fact]
	    public void EncapsulateWithTryFinallyShould_Not_EncapsulateCtor_with_existing_try_finally()
	    {
		    var expectedIl = @".locals init ()
IL_0000: ldarg.0
IL_0001: call System.Void System.Object::.ctor()
IL_0006: ldarg.0
.try
{
IL_0007: ldc.i4.5
IL_0008: stfld System.Int32 AClassWithATryFinallyInConstructor::value
IL_000d: leave.s IL_0016
}
finally
{
IL_000f: ldarg.0
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
		    });
	    }
    }
}