using System.Collections.Generic;
using FluentAssertions;
using MiniCover.Model;

namespace MiniCover.UnitTests.Instrumentation
{
    public class If : BaseTest
    {
        public class Class
        {
            public bool Method(int x)
            {
                if (x % 2 == 0)
                    return true;

                return false;
            }
        }

        public If() : base(typeof(Class).GetMethod(nameof(Class.Method)))
        {
        }

        public override void FunctionalTest()
        {
            new Class().Method(5).Should().Be(false);
            new Class().Method(2).Should().Be(true);
        }

        public override string ExpectedIL => @".locals init (System.Boolean V_0, System.Boolean V_1, MiniCover.HitServices.MethodScope V_2, System.Boolean V_3)
IL_0000: ldstr ""/tmp""
IL_0005: ldstr ""MiniCover.UnitTests""
IL_000a: ldstr ""MiniCover.UnitTests.Instrumentation.If/Class""
IL_000f: ldstr ""Method""
IL_0014: call MiniCover.HitServices.MethodScope MiniCover.HitServices.HitService::EnterMethod(System.String,System.String,System.String,System.String)
IL_0019: stloc.2
IL_001a: nop
.try
{
    IL_001b: nop
    IL_001c: ldloc.2
    IL_001d: ldc.i4.1
    IL_001e: callvirt System.Void MiniCover.HitServices.MethodScope::Hit(System.Int32)
    IL_0023: ldarg.1
    IL_0024: ldc.i4.2
    IL_0025: rem
    IL_0026: ldc.i4.0
    IL_0027: ceq
    IL_0029: stloc.0
    IL_002a: ldloc.0
    IL_002b: brfalse.s IL_0038
    IL_002d: ldloc.2
    IL_002e: ldc.i4.2
    IL_002f: callvirt System.Void MiniCover.HitServices.MethodScope::Hit(System.Int32)
    IL_0034: ldc.i4.1
    IL_0035: stloc.1
    IL_0036: br.s IL_0043
    IL_0038: ldloc.2
    IL_0039: ldc.i4.3
    IL_003a: callvirt System.Void MiniCover.HitServices.MethodScope::Hit(System.Int32)
    IL_003f: ldc.i4.0
    IL_0040: stloc.1
    IL_0041: br.s IL_0043
    IL_0043: ldloc.1
    IL_0044: stloc.3
    IL_0045: leave.s IL_004f
}
finally
{
    IL_0047: nop
    IL_0048: ldloc.2
    IL_0049: callvirt System.Void MiniCover.HitServices.MethodScope::Dispose()
    IL_004e: endfinally
}
IL_004f: ldloc.3
IL_0050: ret
";

        public override IDictionary<int, int> ExpectedHits => new Dictionary<int, int>
        {
            [1] = 2,
            [2] = 1,
            [3] = 1
        };

        public override InstrumentedSequence[] ExpectedInstructions => new InstrumentedSequence[]
        {
            new InstrumentedSequence
            {
                Conditions = new InstrumentedCondition[] {
                    new InstrumentedCondition {
                        Instruction = "IL_0007: stloc V_0",
                        Branches = new InstrumentedBranch[] {
                            new InstrumentedBranch
                            {
                                HitId = 2,
                                External = true,
                                Instruction = "IL_000b: ldc.i4 1",
                            }, new InstrumentedBranch
                            {
                                HitId = 3,
                                External = true,
                                Instruction = "IL_000f: ldc.i4 0"
                            }
                        }
                    }
                },
                Code = "if (x % 2 == 0)",
                EndColumn = 32,
                EndLine = 13,
                HitId = 1,
                Instruction = "IL_0001: ldarg x",
                Method = new InstrumentedMethod
                {
                    Class = "MiniCover.UnitTests.Instrumentation.If/Class",
                    FullName = "System.Boolean MiniCover.UnitTests.Instrumentation.If/Class::Method(System.Int32)",
                    Name = "Method"
                },
                StartColumn = 17,
                StartLine = 13
            },
            new InstrumentedSequence
            {
                Code = "return true;",
                EndColumn = 33,
                EndLine = 14,
                HitId = 2,
                Instruction = "IL_000b: ldc.i4 1",
                Method = new InstrumentedMethod
                {
                    Class = "MiniCover.UnitTests.Instrumentation.If/Class",
                    FullName = "System.Boolean MiniCover.UnitTests.Instrumentation.If/Class::Method(System.Int32)",
                    Name = "Method"
                },
                StartColumn = 21,
                StartLine = 14
            },
            new InstrumentedSequence
            {
                Code = "return false;",
                EndColumn = 30,
                EndLine = 16,
                HitId = 3,
                Instruction = "IL_000f: ldc.i4 0",
                Method = new InstrumentedMethod
                {
                    Class = "MiniCover.UnitTests.Instrumentation.If/Class",
                    FullName = "System.Boolean MiniCover.UnitTests.Instrumentation.If/Class::Method(System.Int32)",
                    Name = "Method"
                },
                StartColumn = 17,
                StartLine = 16
            }
        };
    }
}
