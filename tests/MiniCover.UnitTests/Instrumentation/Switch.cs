using System.Collections.Generic;
using FluentAssertions;
using MiniCover.Model;

namespace MiniCover.UnitTests.Instrumentation
{
    public class Switch : BaseTest
    {
        public class Class
        {
            public int Method(int x)
            {
                switch (x)
                {
                    case 1:
                        return 1;
                    case 2:
                        return 2;
                    case 3:
                        return 3;
                    default:
                        return 0;
                }
            }
        }

        public Switch() : base(typeof(Class).GetMethod(nameof(Class.Method)))
        {
        }

        public override void FunctionalTest()
        {
            new Class().Method(1).Should().Be(1);
            new Class().Method(2).Should().Be(2);
            new Class().Method(3).Should().Be(3);
            new Class().Method(4).Should().Be(0);
        }

        public override string ExpectedIL => @".locals init (System.Int32 V_0, System.Int32 V_1, System.Int32 V_2, MiniCover.HitServices.MethodScope V_3, System.Int32 V_4)
IL_0000: ldstr ""/tmp""
IL_0005: ldstr ""MiniCover.UnitTests""
IL_000a: ldstr ""MiniCover.UnitTests.Instrumentation.Switch/Class""
IL_000f: ldstr ""Method""
IL_0014: call MiniCover.HitServices.MethodScope MiniCover.HitServices.HitService::EnterMethod(System.String,System.String,System.String,System.String)
IL_0019: stloc.3
IL_001a: nop
.try
{
    IL_001b: nop
    IL_001c: ldloc.3
    IL_001d: ldc.i4.1
    IL_001e: callvirt System.Void MiniCover.HitServices.MethodScope::Hit(System.Int32)
    IL_0023: ldarg.1
    IL_0024: stloc.1
    IL_0025: ldloc.1
    IL_0026: stloc.0
    IL_0027: ldloc.0
    IL_0028: ldc.i4.1
    IL_0029: sub
    IL_002a: switch (IL_003d, IL_0048, IL_0053)
    IL_003b: br.s IL_005e
    IL_003d: ldloc.3
    IL_003e: ldc.i4.3
    IL_003f: callvirt System.Void MiniCover.HitServices.MethodScope::Hit(System.Int32)
    IL_0044: ldc.i4.1
    IL_0045: stloc.2
    IL_0046: br.s IL_0069
    IL_0048: ldloc.3
    IL_0049: ldc.i4.4
    IL_004a: callvirt System.Void MiniCover.HitServices.MethodScope::Hit(System.Int32)
    IL_004f: ldc.i4.2
    IL_0050: stloc.2
    IL_0051: br.s IL_0069
    IL_0053: ldloc.3
    IL_0054: ldc.i4.5
    IL_0055: callvirt System.Void MiniCover.HitServices.MethodScope::Hit(System.Int32)
    IL_005a: ldc.i4.3
    IL_005b: stloc.2
    IL_005c: br.s IL_0069
    IL_005e: ldloc.3
    IL_005f: ldc.i4.2
    IL_0060: callvirt System.Void MiniCover.HitServices.MethodScope::Hit(System.Int32)
    IL_0065: ldc.i4.0
    IL_0066: stloc.2
    IL_0067: br.s IL_0069
    IL_0069: ldloc.2
    IL_006a: stloc.s V_4
    IL_006c: leave.s IL_0076
}
finally
{
    IL_006e: nop
    IL_006f: ldloc.3
    IL_0070: callvirt System.Void MiniCover.HitServices.MethodScope::Dispose()
    IL_0075: endfinally
}
IL_0076: ldloc.s V_4
IL_0078: ret
";

        public override IDictionary<int, int> ExpectedHits => new Dictionary<int, int>
        {
            [1] = 4,
            [2] = 1,
            [3] = 1,
            [4] = 1,
            [5] = 1
        };

        public override InstrumentedSequence[] ExpectedInstructions => new InstrumentedSequence[]
        {
            new InstrumentedSequence
            {
                Conditions = new InstrumentedCondition[] {
                    new InstrumentedCondition {
                        Instruction = "IL_0002: stloc V_1",
                        Branches = new InstrumentedBranch[] {
                            new InstrumentedBranch
                            {
                                HitId = 2,
                                External = true,
                                Instruction = "IL_0027: ldc.i4 0"
                            },
                            new InstrumentedBranch
                            {
                                HitId = 3,
                                External = true,
                                Instruction = "IL_001b: ldc.i4 1"
                            },
                            new InstrumentedBranch
                            {
                                HitId = 4,
                                External = true,
                                Instruction = "IL_001f: ldc.i4 2"
                            },
                            new InstrumentedBranch
                            {
                                HitId = 5,
                                External = true,
                                Instruction = "IL_0023: ldc.i4 3"
                            }
                        }
                    }
                },
                Code = "switch (x)",
                EndColumn = 27,
                EndLine = 13,
                HitId = 1,
                Instruction = "IL_0001: ldarg x",
                Method = new InstrumentedMethod
                {
                    Class = "MiniCover.UnitTests.Instrumentation.Switch/Class",
                    FullName = "System.Int32 MiniCover.UnitTests.Instrumentation.Switch/Class::Method(System.Int32)",
                    Name = "Method"
                },
                StartColumn = 17,
                StartLine = 13
            },
            new InstrumentedSequence
            {
                Code = "return 1;",
                EndColumn = 34,
                EndLine = 16,
                HitId = 3,
                Instruction = "IL_001b: ldc.i4 1",
                Method = new InstrumentedMethod
                {
                    Class = "MiniCover.UnitTests.Instrumentation.Switch/Class",
                    FullName = "System.Int32 MiniCover.UnitTests.Instrumentation.Switch/Class::Method(System.Int32)",
                    Name = "Method"
                },
                StartColumn = 25,
                StartLine = 16
            },
            new InstrumentedSequence
            {
                Code = "return 2;",
                EndColumn = 34,
                EndLine = 18,
                HitId = 4,
                Instruction = "IL_001f: ldc.i4 2",
                Method = new InstrumentedMethod
                {
                    Class = "MiniCover.UnitTests.Instrumentation.Switch/Class",
                    FullName = "System.Int32 MiniCover.UnitTests.Instrumentation.Switch/Class::Method(System.Int32)",
                    Name = "Method"
                },
                StartColumn = 25,
                StartLine = 18
            },
            new InstrumentedSequence
            {
                Code = "return 3;",
                EndColumn = 34,
                EndLine = 20,
                HitId = 5,
                Instruction = "IL_0023: ldc.i4 3",
                Method = new InstrumentedMethod
                {
                    Class = "MiniCover.UnitTests.Instrumentation.Switch/Class",
                    FullName = "System.Int32 MiniCover.UnitTests.Instrumentation.Switch/Class::Method(System.Int32)",
                    Name = "Method"
                },
                StartColumn = 25,
                StartLine = 20
            },
            new InstrumentedSequence
            {
                Code = "return 0;",
                EndColumn = 34,
                EndLine = 22,
                HitId = 2,
                Instruction = "IL_0027: ldc.i4 0",
                Method = new InstrumentedMethod
                {
                    Class = "MiniCover.UnitTests.Instrumentation.Switch/Class",
                    FullName = "System.Int32 MiniCover.UnitTests.Instrumentation.Switch/Class::Method(System.Int32)",
                    Name = "Method"
                },
                StartColumn = 25,
                StartLine = 22
            }
        };
    }
}
