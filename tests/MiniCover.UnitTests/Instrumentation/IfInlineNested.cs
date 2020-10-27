using System.Collections.Generic;
using FluentAssertions;
using MiniCover.Model;

namespace MiniCover.UnitTests.Instrumentation
{
    public class IfInlineNested : BaseTest
    {
        public class Class
        {
            public bool Method(int x)
            {
                return x % 2 == 0
                    ? true
                    : x % 3 == 0
                        ? true
                        : false;
            }
        }

        public IfInlineNested() : base(typeof(Class).GetMethod(nameof(Class.Method)))
        {
        }

        public override void FunctionalTest()
        {
            new Class().Method(2).Should().Be(true);
            new Class().Method(3).Should().Be(true);
            new Class().Method(5).Should().Be(false);
        }

        public override string ExpectedIL => @".locals init (System.Boolean V_0, MiniCover.HitServices.MethodScope V_1, System.Boolean V_2)
IL_0000: ldstr ""/tmp""
IL_0005: ldstr ""MiniCover.UnitTests""
IL_000a: ldstr ""MiniCover.UnitTests.Instrumentation.IfInlineNested/Class""
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
    IL_0026: brfalse.s IL_0048
    IL_0028: ldloc.1
    IL_0029: ldc.i4.2
    IL_002a: callvirt System.Void MiniCover.HitServices.MethodScope::Hit(System.Int32)
    IL_002f: ldarg.1
    IL_0030: ldc.i4.3
    IL_0031: rem
    IL_0032: brfalse.s IL_003e
    IL_0034: ldloc.1
    IL_0035: ldc.i4.4
    IL_0036: callvirt System.Void MiniCover.HitServices.MethodScope::Hit(System.Int32)
    IL_003b: ldc.i4.0
    IL_003c: br.s IL_0046
    IL_003e: ldloc.1
    IL_003f: ldc.i4.5
    IL_0040: callvirt System.Void MiniCover.HitServices.MethodScope::Hit(System.Int32)
    IL_0045: ldc.i4.1
    IL_0046: br.s IL_0050
    IL_0048: ldloc.1
    IL_0049: ldc.i4.3
    IL_004a: callvirt System.Void MiniCover.HitServices.MethodScope::Hit(System.Int32)
    IL_004f: ldc.i4.1
    IL_0050: stloc.0
    IL_0051: br.s IL_0053
    IL_0053: ldloc.0
    IL_0054: stloc.2
    IL_0055: leave.s IL_005f
}
finally
{
    IL_0057: nop
    IL_0058: ldloc.1
    IL_0059: callvirt System.Void MiniCover.HitServices.MethodScope::Dispose()
    IL_005e: endfinally
}
IL_005f: ldloc.2
IL_0060: ret
";

        public override IDictionary<int, int> ExpectedHits => new Dictionary<int, int>
        {
            [1] = 3,
            [2] = 2,
            [3] = 1,
            [4] = 1,
            [5] = 1
        };


        public override InstrumentedSequence[] ExpectedInstructions => new InstrumentedSequence[]
        {
            new InstrumentedSequence
            {
                Code = @"return x % 2 == 0
                    ? true
                    : x % 3 == 0
                        ? true
                        : false;",
                EndColumn = 33,
                EndLine = 17,
                HitId = 1,
                Instruction = "IL_0001: ldarg x",
                Method = new InstrumentedMethod
                {
                    Class = "MiniCover.UnitTests.Instrumentation.IfInlineNested/Class",
                    FullName = "System.Boolean MiniCover.UnitTests.Instrumentation.IfInlineNested/Class::Method(System.Int32)",
                    Name = "Method",
                },
                StartColumn = 17,
                StartLine = 13,
                Conditions = new InstrumentedCondition[] {
                    new InstrumentedCondition {
                        Instruction = "IL_0004: brfalse IL_0000",
                        Branches = new InstrumentedBranch[]
                        {
                            new InstrumentedBranch
                            {
                                HitId = 2,
                                Instruction = "IL_0006: ldarg x"
                            },
                            new InstrumentedBranch
                            {
                                HitId = 3,
                                Instruction = "IL_0011: ldc.i4 1"
                            }
                        }
                    },
                    new InstrumentedCondition {
                        Instruction = "IL_0009: brfalse IL_0000",
                        Branches = new InstrumentedBranch[]
                        {
                            new InstrumentedBranch
                            {
                                HitId = 4,
                                Instruction = "IL_000b: ldc.i4 0"
                            },
                            new InstrumentedBranch
                            {
                                HitId = 5,
                                Instruction = "IL_000e: ldc.i4 1"
                            }
                        }
                    }
                }
            }
        };
    }
}
