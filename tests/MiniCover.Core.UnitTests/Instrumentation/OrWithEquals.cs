using System.Collections.Generic;
using FluentAssertions;
using MiniCover.Core.Model;

namespace MiniCover.Core.UnitTests.Instrumentation
{
    public class OrWithEquals : BaseTest
    {
        public class Class
        {
            public bool Method(int x)
            {
                return x % 2 == 0 || x % 3 == 0;
            }
        }

        public OrWithEquals() : base(typeof(Class).GetMethod(nameof(Class.Method)))
        {
        }

        public override void FunctionalTest()
        {
            new Class().Method(2).Should().Be(true);
            new Class().Method(5).Should().Be(false);
        }

        public override string ExpectedIL => @".locals init (System.Boolean V_0, MiniCover.HitServices.MethodScope V_1, System.Boolean V_2)
IL_0000: ldstr ""/tmp""
IL_0005: ldstr ""MiniCover.Core.UnitTests""
IL_000a: ldstr ""MiniCover.Core.UnitTests.Instrumentation.OrWithEquals/Class""
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
    IL_0024: ldc.i4.2
    IL_0025: rem
    IL_0026: brfalse.s IL_0037
    IL_0028: ldloc.1
    IL_0029: ldc.i4.2
    IL_002a: callvirt System.Void MiniCover.HitServices.MethodScope::Hit(System.Int32)
    IL_002f: ldarg.1
    IL_0030: ldc.i4.3
    IL_0031: rem
    IL_0032: ldc.i4.0
    IL_0033: ceq
    IL_0035: br.s IL_003f
    IL_0037: ldloc.1
    IL_0038: ldc.i4.3
    IL_0039: callvirt System.Void MiniCover.HitServices.MethodScope::Hit(System.Int32)
    IL_003e: ldc.i4.1
    IL_003f: stloc.0
    IL_0040: br.s IL_0042
    IL_0042: ldloc.0
    IL_0043: stloc.2
    IL_0044: leave.s IL_004e
}
finally
{
    IL_0046: nop
    IL_0047: ldloc.1
    IL_0048: callvirt System.Void MiniCover.HitServices.MethodScope::Dispose()
    IL_004d: endfinally
}
IL_004e: ldloc.2
IL_004f: ret
";

        public override IDictionary<int, int> ExpectedHits => new Dictionary<int, int>
        {
            [1] = 2,
            [3] = 1,
            [2] = 1
        };

        public override InstrumentedSequence[] ExpectedInstructions => new InstrumentedSequence[]
        {
            new InstrumentedSequence
            {
                Code = "return x % 2 == 0 || x % 3 == 0;",
                EndColumn = 49,
                EndLine = 13,
                HitId = 1,
                Instruction = "IL_0001: ldarg x",
                Method = new InstrumentedMethod
                {
                    Class = "MiniCover.Core.UnitTests.Instrumentation.OrWithEquals/Class",
                    FullName = "System.Boolean MiniCover.Core.UnitTests.Instrumentation.OrWithEquals/Class::Method(System.Int32)",
                    Name = "Method"
                },
                StartColumn = 17,
                StartLine = 13,
                Conditions = new InstrumentedCondition[]
                {
                    new InstrumentedCondition
                    {
                        Instruction = "IL_0004: brfalse IL_0000",
                        Branches = new InstrumentedBranch[] {
                            new InstrumentedBranch
                            {
                                External = false,
                                HitId = 2,
                                Instruction = "IL_0006: ldarg x"
                            },
                            new InstrumentedBranch
                            {
                                External = false,
                                HitId = 3,
                                Instruction = "IL_000e: ldc.i4 1"
                            }
                        }
                    }
                }
            }
        };
    }
}
