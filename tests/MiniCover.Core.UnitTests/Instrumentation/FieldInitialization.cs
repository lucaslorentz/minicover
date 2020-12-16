using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace MiniCover.Core.UnitTests.Instrumentation
{
    public class FieldInitialization : BaseTest
    {
        public class Class
        {
            public readonly int Value = 5;
        }

        public FieldInitialization() : base(typeof(Class).GetConstructors().First())
        {
        }

        public override void FunctionalTest()
        {
            var result = new Class();
            result.Value.Should().Be(5);
        }

        public override string ExpectedIL => @".locals init (MiniCover.HitServices.MethodScope V_0)
IL_0000: ldstr ""/tmp""
IL_0005: ldstr ""MiniCover.Core.UnitTests""
IL_000a: ldstr ""MiniCover.Core.UnitTests.Instrumentation.FieldInitialization/Class""
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
    IL_0023: ldc.i4.5
    IL_0024: stfld System.Int32 MiniCover.Core.UnitTests.Instrumentation.FieldInitialization/Class::Value
    IL_0029: ldarg.0 // this
    IL_002a: call System.Void System.Object::.ctor()
    IL_002f: nop
    IL_0030: leave.s IL_003a
}
finally
{
    IL_0032: nop
    IL_0033: ldloc.0
    IL_0034: callvirt System.Void MiniCover.HitServices.MethodScope::Dispose()
    IL_0039: endfinally
}
IL_003a: ret
";

        public override IDictionary<int, int> ExpectedHits => new Dictionary<int, int>
        {
            [1] = 1
        };
    }
}
