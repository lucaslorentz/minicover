using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace MiniCover.Core.UnitTests.Instrumentation
{
    public class ConstructorWithCallBase : BaseTest
    {
        public class Class : BaseClass
        {
            public readonly bool WasCalled;

            public Class(int x) : base(x)
            {
                WasCalled = true;
            }
        }

        public class BaseClass
        {
            public readonly bool BaseWasCalled;

            public BaseClass(int x)
            {
                BaseWasCalled = true;
            }
        }

        public ConstructorWithCallBase() : base(typeof(Class).GetConstructors().First())
        {
        }

        public override void FunctionalTest()
        {
            var result = new Class(5);
            result.WasCalled.Should().BeTrue();
            result.BaseWasCalled.Should().BeTrue();
        }

        public override string ExpectedIL => @".locals init (MiniCover.HitServices.MethodScope V_0)
IL_0000: ldstr ""/tmp""
IL_0005: ldstr ""MiniCover.Core.UnitTests""
IL_000a: ldstr ""MiniCover.Core.UnitTests.Instrumentation.ConstructorWithCallBase/Class""
IL_000f: ldstr "".ctor""
IL_0014: call MiniCover.HitServices.MethodScope MiniCover.HitServices.HitService::EnterMethod(System.String,System.String,System.String,System.String)
IL_0019: stloc.0
IL_001a: nop
.try
{
    IL_001b: ldloc.0
    IL_001c: ldc.i4.1
    IL_001d: callvirt System.Void MiniCover.HitServices.MethodScope::Hit(System.Int32)
    IL_0022: ldarg.0 // this
    IL_0023: ldarg.1
    IL_0024: call System.Void MiniCover.Core.UnitTests.Instrumentation.ConstructorWithCallBase/BaseClass::.ctor(System.Int32)
    IL_0029: nop
    IL_002a: nop
    IL_002b: ldloc.0
    IL_002c: ldc.i4.2
    IL_002d: callvirt System.Void MiniCover.HitServices.MethodScope::Hit(System.Int32)
    IL_0032: ldarg.0 // this
    IL_0033: ldc.i4.1
    IL_0034: stfld System.Boolean MiniCover.Core.UnitTests.Instrumentation.ConstructorWithCallBase/Class::WasCalled
    IL_0039: leave.s IL_0043
}
finally
{
    IL_003b: nop
    IL_003c: ldloc.0
    IL_003d: callvirt System.Void MiniCover.HitServices.MethodScope::Dispose()
    IL_0042: endfinally
}
IL_0043: ret
";

        public override IDictionary<int, int> ExpectedHits => new Dictionary<int, int>
        {
            [1] = 1,
            [2] = 1
        };
    }
}