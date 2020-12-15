using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using MiniCover.Core.Model;

namespace MiniCover.Core.UnitTests.Instrumentation
{
    public class ExcludedFromCodeCoverageClass : BaseTest
    {
        [ExcludeFromCodeCoverage]
        public class Class
        {
            public int Method(int value)
            {
                return value * 2;
            }
        }

        public ExcludedFromCodeCoverageClass() : base(typeof(Class).GetMethod(nameof(Class.Method)))
        {
        }

        public override void FunctionalTest()
        {
            new Class().Method(5).Should().Be(10);
        }

        public override string ExpectedIL => @".locals init (System.Int32 V_0, MiniCover.HitServices.MethodScope V_1, System.Int32 V_2)
IL_0000: ldstr ""/tmp""
IL_0005: ldstr ""MiniCover.Core.UnitTests""
IL_000a: ldstr ""MiniCover.Core.UnitTests.Instrumentation.ExcludedFromCodeCoverageClass/Class""
IL_000f: ldstr ""Method""
IL_0014: call MiniCover.HitServices.MethodScope MiniCover.HitServices.HitService::EnterMethod(System.String,System.String,System.String,System.String)
IL_0019: stloc.1
IL_001a: nop
.try
{
    IL_001b: nop
    IL_001c: ldarg.1
    IL_001d: ldc.i4.2
    IL_001e: mul
    IL_001f: stloc.0
    IL_0020: br.s IL_0022
    IL_0022: ldloc.0
    IL_0023: stloc.2
    IL_0024: leave.s IL_002e
}
finally
{
    IL_0026: nop
    IL_0027: ldloc.1
    IL_0028: callvirt System.Void MiniCover.HitServices.MethodScope::Dispose()
    IL_002d: endfinally
}
IL_002e: ldloc.2
IL_002f: ret
";

        public override IDictionary<int, int> ExpectedHits => new Dictionary<int, int>();

        public override InstrumentedSequence[] ExpectedInstructions => new InstrumentedSequence[0];
    }
}
