using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace MiniCover.UnitTests.Instrumentation
{
    public class ConstructorWithExistingTryFinally : BaseTest
    {
        public class Class
        {
            public readonly bool TryWasCalled;
            public readonly bool CatchWasCalled;
            public readonly bool FinallyWasCalled;

            public Class()
            {
                try
                {
                    TryWasCalled = true;
                    throw new Exception("Test");
                }
                catch (Exception)
                {
                    CatchWasCalled = true;
                }
                finally
                {
                    FinallyWasCalled = true;
                }
            }
        }

        public ConstructorWithExistingTryFinally() : base(typeof(Class).GetConstructors().First())
        {
        }

        public override void FunctionalTest()
        {
            var result = new Class();
            result.TryWasCalled.Should().BeTrue();
            result.CatchWasCalled.Should().BeTrue();
            result.FinallyWasCalled.Should().BeTrue();
        }

        public override string ExpectedIL => @".locals init (MiniCover.HitServices.HitService/MethodContext V_0)
IL_0000: ldstr ""/tmp""
IL_0005: ldstr ""MiniCover.UnitTests""
IL_000a: ldstr ""MiniCover.UnitTests.Instrumentation.ConstructorWithExistingTryFinally/Class""
IL_000f: ldstr "".ctor""
IL_0014: call MiniCover.HitServices.HitService/MethodContext MiniCover.HitServices.HitService::EnterMethod(System.String,System.String,System.String,System.String)
IL_0019: stloc.0
IL_001a: nop
.try
{
    IL_001b: ldloc.0
    IL_001c: ldc.i4.1
    IL_001d: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Hit(System.Int32)
    IL_0022: ldarg.0 // this
    IL_0023: call System.Void System.Object::.ctor()
    IL_0028: nop
    IL_0029: nop
    .try
    {
        .try
        {
            IL_002a: nop
            IL_002b: ldloc.0
            IL_002c: ldc.i4.2
            IL_002d: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Hit(System.Int32)
            IL_0032: ldarg.0 // this
            IL_0033: ldc.i4.1
            IL_0034: stfld System.Boolean MiniCover.UnitTests.Instrumentation.ConstructorWithExistingTryFinally/Class::TryWasCalled
            IL_0039: ldloc.0
            IL_003a: ldc.i4.3
            IL_003b: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Hit(System.Int32)
            IL_0040: ldstr ""Test""
            IL_0045: newobj System.Void System.Exception::.ctor(System.String)
            IL_004a: throw
        }
        catch System.Exception
        {
            IL_004b: ldloc.0
            IL_004c: ldc.i4.4
            IL_004d: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Hit(System.Int32)
            IL_0052: pop
            IL_0053: nop
            IL_0054: ldloc.0
            IL_0055: ldc.i4.5
            IL_0056: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Hit(System.Int32)
            IL_005b: ldarg.0 // this
            IL_005c: ldc.i4.1
            IL_005d: stfld System.Boolean MiniCover.UnitTests.Instrumentation.ConstructorWithExistingTryFinally/Class::CatchWasCalled
            IL_0062: nop
            IL_0063: leave.s IL_0065
        }
        IL_0065: leave.s IL_0078
    }
    finally
    {
        IL_0067: nop
        IL_0068: ldloc.0
        IL_0069: ldc.i4.6
        IL_006a: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Hit(System.Int32)
        IL_006f: ldarg.0 // this
        IL_0070: ldc.i4.1
        IL_0071: stfld System.Boolean MiniCover.UnitTests.Instrumentation.ConstructorWithExistingTryFinally/Class::FinallyWasCalled
        IL_0076: nop
        IL_0077: endfinally
    }
    IL_0078: leave.s IL_0082
}
finally
{
    IL_007a: nop
    IL_007b: ldloc.0
    IL_007c: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Dispose()
    IL_0081: endfinally
}
IL_0082: ret
";

        public override IDictionary<int, int> ExpectedHits => new Dictionary<int, int>
        {
            [1] = 1,
            [2] = 1,
            [3] = 1,
            [4] = 1,
            [5] = 1,
            [6] = 1
        };
    }
}