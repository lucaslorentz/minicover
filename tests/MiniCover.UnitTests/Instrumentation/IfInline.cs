using System.Collections.Generic;
using FluentAssertions;
using MiniCover.Model;

namespace MiniCover.UnitTests.Instrumentation
{
    public class IfInline : BaseTest
    {
        public class Class
        {
            public bool Method(int x)
            {
                return x % 2 == 0
                    ? true
                    : false;
            }
        }

        public IfInline() : base(typeof(Class).GetMethod(nameof(Class.Method)))
        {
        }

        public override void FunctionalTest()
        {
            new Class().Method(5).Should().Be(false);
            new Class().Method(2).Should().Be(true);
        }

        public override string ExpectedIL => @".locals init (System.Boolean V_0, MiniCover.HitServices.HitService/MethodContext V_1, System.Boolean V_2)
IL_0000: ldstr ""/tmp""
IL_0005: ldstr ""MiniCover.UnitTests""
IL_000a: ldstr ""MiniCover.UnitTests.Instrumentation.IfInline/Class""
IL_000f: ldstr ""Method""
IL_0014: call MiniCover.HitServices.HitService/MethodContext MiniCover.HitServices.HitService::EnterMethod(System.String,System.String,System.String,System.String)
IL_0019: stloc.1
IL_001a: nop
.try
{
    IL_001b: nop
    IL_001c: ldloc.1
    IL_001d: ldc.i4.1
    IL_001e: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Hit(System.Int32)
    IL_0023: ldarg.1
    IL_0024: ldc.i4.2
    IL_0025: rem
    IL_0026: brfalse.s IL_0032
    IL_0028: ldloc.1
    IL_0029: ldc.i4.2
    IL_002a: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Hit(System.Int32)
    IL_002f: ldc.i4.0
    IL_0030: br.s IL_003a
    IL_0032: ldloc.1
    IL_0033: ldc.i4.3
    IL_0034: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Hit(System.Int32)
    IL_0039: ldc.i4.1
    IL_003a: stloc.0
    IL_003b: br.s IL_003d
    IL_003d: ldloc.0
    IL_003e: stloc.2
    IL_003f: leave.s IL_0049
}
finally
{
    IL_0041: nop
    IL_0042: ldloc.1
    IL_0043: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Dispose()
    IL_0048: endfinally
}
IL_0049: ldloc.2
IL_004a: ret
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
                Code = @"return x % 2 == 0
                    ? true
                    : false;",
                EndColumn = 29,
                EndLine = 15,
                HitId = 1,
                Instruction = "IL_0001: ldarg x",
                Method = new InstrumentedMethod
                {
                    Class = "MiniCover.UnitTests.Instrumentation.IfInline/Class",
                    FullName = "System.Boolean MiniCover.UnitTests.Instrumentation.IfInline/Class::Method(System.Int32)",
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
                                Instruction = "IL_0006: ldc.i4 0",
                            },
                            new InstrumentedBranch
                            {
                                HitId = 3,
                                Instruction = "IL_0009: ldc.i4 1"
                            }
                        }
                    }
                }
            }
        };
    }
}
