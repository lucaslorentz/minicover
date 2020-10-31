using System.Collections.Generic;
using FluentAssertions;
using MiniCover.Model;

namespace MiniCover.UnitTests.Instrumentation
{
    public class Or : BaseTest
    {
        public class Class
        {
            public bool Method(bool x, bool y)
            {
                return x || y;
            }
        }

        public Or() : base(typeof(Class).GetMethod(nameof(Class.Method)))
        {
        }

        public override void FunctionalTest()
        {
            new Class().Method(false, false).Should().Be(false);
            new Class().Method(true, false).Should().Be(true);
        }

        public override string ExpectedIL => @".locals init (System.Boolean V_0, MiniCover.HitServices.MethodScope V_1, System.Boolean V_2)
IL_0000: ldstr ""/tmp""
IL_0005: ldstr ""MiniCover.UnitTests""
IL_000a: ldstr ""MiniCover.UnitTests.Instrumentation.Or/Class""
IL_000f: ldstr ""Method""
IL_0014: call MiniCover.HitServices.MethodScope MiniCover.HitServices.HitService::EnterMethod(System.String,System.String,System.String,System.String)
IL_0019: stloc.1
IL_001a: nop
.try
{
    IL_001b: nop
    IL_001c: ldloc.1
    IL_001d: ldc.i4.1
    IL_001e: callvirt System.Void MiniCover.HitServices.MethodScope::Hit(System.Int32)
    IL_0023: ldarg.1
    IL_0024: ldarg.2
    IL_0025: or
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
    IL_002f: callvirt System.Void MiniCover.HitServices.MethodScope::Dispose()
    IL_0034: endfinally
}
IL_0035: ldloc.2
IL_0036: ret
";

        public override IDictionary<int, int> ExpectedHits => new Dictionary<int, int>
        {
            [1] = 2
        };

        public override InstrumentedSequence[] ExpectedInstructions => new InstrumentedSequence[]
        {
            new InstrumentedSequence
            {
                Code = "return x || y;",
                EndColumn = 31,
                EndLine = 13,
                HitId = 1,
                Instruction = "IL_0001: ldarg x",
                Method = new InstrumentedMethod
                {
                    Class = "MiniCover.UnitTests.Instrumentation.Or/Class",
                    FullName = "System.Boolean MiniCover.UnitTests.Instrumentation.Or/Class::Method(System.Boolean,System.Boolean)",
                    Name = "Method"
                },
                StartColumn = 17,
                StartLine = 13
            }
        };
    }
}
