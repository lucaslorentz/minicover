using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace MiniCover.UnitTests.Instrumentation
{
    public class ConstructorGenericCallThis : BaseTest
    {
        public class Class<T>
        {
            public readonly T Value;
            public readonly bool Other;

            public Class(T value)
                : this(value, true)
            {
            }

            public Class(T value, bool other)
            {
                Value = value;
                Other = other;
            }
        }

        public ConstructorGenericCallThis() : base(typeof(Class<>).GetConstructors().First())
        {
        }

        public override void FunctionalTest()
        {
            var result = new Class<int>(5);
            result.Value.Should().Be(5);
            result.Other.Should().BeTrue();
        }

        public override string ExpectedIL => @".locals init (MiniCover.HitServices.MethodScope V_0)
IL_0000: ldstr ""/tmp""
IL_0005: ldstr ""MiniCover.UnitTests""
IL_000a: ldstr ""MiniCover.UnitTests.Instrumentation.ConstructorGenericCallThis/Class`1""
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
    IL_0024: ldc.i4.1
    IL_0025: call System.Void MiniCover.UnitTests.Instrumentation.ConstructorGenericCallThis/Class`1<T>::.ctor(T,System.Boolean)
    IL_002a: nop
    IL_002b: nop
    IL_002c: leave.s IL_0036
}
finally
{
    IL_002e: nop
    IL_002f: ldloc.0
    IL_0030: callvirt System.Void MiniCover.HitServices.MethodScope::Dispose()
    IL_0035: endfinally
}
IL_0036: ret
";

        public override IDictionary<int, int> ExpectedHits => new Dictionary<int, int>
        {
            [1] = 1
        };
    }
}
