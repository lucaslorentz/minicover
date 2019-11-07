using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace MiniCover.UnitTests.Instrumentation
{
    public class StaticMethod : BaseInstrumentationTest
    {
        public static class Class
        {
            public static int Method(int value)
            {
                return value * 2;
            }
        }

        public StaticMethod() : base(typeof(Class).GetMethod(nameof(Class.Method)))
        {
        }

        public override void FunctionalTest()
        {
            Class.Method(5).Should().Be(10);
        }

        public override string ExpectedIL => @".locals init (System.Int32 V_0, MiniCover.HitServices.HitService/MethodContext V_1, System.Int32 V_2)
IL_0000: ldstr ""/tmp""
IL_0005: ldstr ""MiniCover.UnitTests""
IL_000a: ldstr ""MiniCover.UnitTests.Instrumentation.StaticMethod/Class""
IL_000f: ldstr ""Method""
IL_0014: call MiniCover.HitServices.HitService/MethodContext MiniCover.HitServices.HitService::EnterMethod(System.String,System.String,System.String,System.String)
IL_0019: stloc.1
IL_001a: nop
.try
{
    IL_001b: nop
    IL_001c: ldloc.1
    IL_001d: ldc.i4.1
    IL_001e: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::HitInstruction(System.Int32)
    IL_0023: ldarg.0 // this
    IL_0024: ldc.i4.2
    IL_0025: mul
    IL_0026: stloc.0
    IL_0027: br.s IL_0029
    IL_0029: ldloc.0
    IL_002a: stloc.2
    IL_002b: leave.s IL_0035
}
finally
{
    IL_002d: nop
    IL_002e: ldloc.1
    IL_002f: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Dispose()
    IL_0034: endfinally
}
IL_0035: ldloc.2
IL_0036: ret
";

        public override IDictionary<int, int> ExpectedHits => new Dictionary<int, int>
        {
            [1] = 1
        };
    }
}
