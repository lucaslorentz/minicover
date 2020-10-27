using System.Collections.Generic;
using FluentAssertions;
using MiniCover.Model;

namespace MiniCover.UnitTests.Instrumentation
{
    public class For : BaseTest
    {
        public class Class
        {
            public int Method(int x)
            {
                var result = 0;
                for (var i = 1; i <= x; i++)
                    result += i;
                return result;
            }
        }

        public For() : base(typeof(Class).GetMethod(nameof(Class.Method)))
        {
        }

        public override void FunctionalTest()
        {
            new Class().Method(5).Should().Be(5 + 4 + 3 + 2 + 1);
        }

        public override string ExpectedIL => @".locals init (System.Int32 result, System.Int32 i, System.Boolean V_2, System.Int32 V_3, MiniCover.HitServices.MethodScope V_4, System.Int32 V_5)
IL_0000: ldstr ""/tmp""
IL_0005: ldstr ""MiniCover.UnitTests""
IL_000a: ldstr ""MiniCover.UnitTests.Instrumentation.For/Class""
IL_000f: ldstr ""Method""
IL_0014: call MiniCover.HitServices.MethodScope MiniCover.HitServices.HitService::EnterMethod(System.String,System.String,System.String,System.String)
IL_0019: stloc.s V_4
IL_001b: nop
.try
{
    IL_001c: nop
    IL_001d: ldloc.s V_4
    IL_001f: ldc.i4.1
    IL_0020: callvirt System.Void MiniCover.HitServices.MethodScope::Hit(System.Int32)
    IL_0025: ldc.i4.0
    IL_0026: stloc.0
    IL_0027: ldloc.s V_4
    IL_0029: ldc.i4.2
    IL_002a: callvirt System.Void MiniCover.HitServices.MethodScope::Hit(System.Int32)
    IL_002f: ldc.i4.1
    IL_0030: stloc.1
    IL_0031: br.s IL_004b
    IL_0033: ldloc.s V_4
    IL_0035: ldc.i4.3
    IL_0036: callvirt System.Void MiniCover.HitServices.MethodScope::Hit(System.Int32)
    IL_003b: ldloc.0
    IL_003c: ldloc.1
    IL_003d: add
    IL_003e: stloc.0
    IL_003f: ldloc.s V_4
    IL_0041: ldc.i4.4
    IL_0042: callvirt System.Void MiniCover.HitServices.MethodScope::Hit(System.Int32)
    IL_0047: ldloc.1
    IL_0048: ldc.i4.1
    IL_0049: add
    IL_004a: stloc.1
    IL_004b: ldloc.s V_4
    IL_004d: ldc.i4.5
    IL_004e: callvirt System.Void MiniCover.HitServices.MethodScope::Hit(System.Int32)
    IL_0053: ldloc.1
    IL_0054: ldarg.1
    IL_0055: cgt
    IL_0057: ldc.i4.0
    IL_0058: ceq
    IL_005a: stloc.2
    IL_005b: ldloc.2
    IL_005c: brtrue.s IL_0033
    IL_005e: ldloc.s V_4
    IL_0060: ldc.i4.6
    IL_0061: callvirt System.Void MiniCover.HitServices.MethodScope::Hit(System.Int32)
    IL_0066: ldloc.0
    IL_0067: stloc.3
    IL_0068: br.s IL_006a
    IL_006a: ldloc.3
    IL_006b: stloc.s V_5
    IL_006d: leave.s IL_0078
}
finally
{
    IL_006f: nop
    IL_0070: ldloc.s V_4
    IL_0072: callvirt System.Void MiniCover.HitServices.MethodScope::Dispose()
    IL_0077: endfinally
}
IL_0078: ldloc.s V_5
IL_007a: ret
";

        public override IDictionary<int, int> ExpectedHits => new Dictionary<int, int>
        {
            [1] = 1,
            [2] = 1,
            [3] = 5,
            [4] = 5,
            [5] = 6,
            [6] = 1
        };

        public override InstrumentedSequence[] ExpectedInstructions => new InstrumentedSequence[]
        {
            new InstrumentedSequence
            {
                Code = "var result = 0;",
                EndColumn = 32,
                EndLine = 13,
                HitId = 1,
                Instruction = "IL_0001: ldc.i4 0",
                Method = new InstrumentedMethod
                {
                    Class = "MiniCover.UnitTests.Instrumentation.For/Class",
                    FullName = "System.Int32 MiniCover.UnitTests.Instrumentation.For/Class::Method(System.Int32)",
                    Name = "Method"
                },
                StartColumn = 17,
                StartLine = 13
            },
            new InstrumentedSequence
            {
                Code = "var i = 1",
                EndColumn = 31,
                EndLine = 14,
                HitId = 2,
                Instruction = "IL_0003: ldc.i4 1",
                Method =new InstrumentedMethod
                {
                    Class = "MiniCover.UnitTests.Instrumentation.For/Class",
                    FullName = "System.Int32 MiniCover.UnitTests.Instrumentation.For/Class::Method(System.Int32)",
                    Name = "Method"
                },
                StartColumn = 22,
                StartLine = 14
            },
            new InstrumentedSequence
            {
                Code = "result += i;",
                EndColumn = 33,
                EndLine = 15,
                HitId = 3,
                Instruction = "IL_0007: ldloc V_0",
                Method =new InstrumentedMethod
                {
                    Class = "MiniCover.UnitTests.Instrumentation.For/Class",
                    FullName = "System.Int32 MiniCover.UnitTests.Instrumentation.For/Class::Method(System.Int32)",
                    Name = "Method"
                },
                StartColumn = 21,
                StartLine = 15,
            },
            new InstrumentedSequence
            {
                Code = "i++",
                EndColumn = 44,
                EndLine = 14,
                HitId = 4,
                Instruction = "IL_000b: ldloc V_1",
                Method = new InstrumentedMethod
                {
                    Class = "MiniCover.UnitTests.Instrumentation.For/Class",
                    FullName = "System.Int32 MiniCover.UnitTests.Instrumentation.For/Class::Method(System.Int32)",
                    Name = "Method"
                },
                StartColumn = 41,
                StartLine = 14
            },
            new InstrumentedSequence
            {
                Code = "i <= x",
                Conditions = new InstrumentedCondition[] {
                    new InstrumentedCondition
                    {
                        Instruction = "IL_0016: stloc V_2",
                        Branches = new InstrumentedBranch[] {
                            new InstrumentedBranch
                            {
                                External = true,
                                HitId = 6,
                                Instruction = "IL_001a: ldloc V_0"
                            },
                            new InstrumentedBranch
                            {
                                External = true,
                                HitId = 3,
                                Instruction = "IL_0007: ldloc V_0"
                            }
                        }
                    }
                },
                EndColumn = 39,
                EndLine = 14,
                HitId = 5,
                Instruction = "IL_000f: ldloc V_1",
                Method =new InstrumentedMethod
                {
                    Class = "MiniCover.UnitTests.Instrumentation.For/Class",
                    FullName = "System.Int32 MiniCover.UnitTests.Instrumentation.For/Class::Method(System.Int32)",
                    Name = "Method"
                },
                StartColumn = 33,
                StartLine = 14,
            },
            new InstrumentedSequence
            {
                Code = "return result;",
                EndColumn = 31,
                EndLine = 16,
                HitId = 6,
                Instruction = "IL_001a: ldloc V_0",
                Method = new InstrumentedMethod
                {
                    Class = "MiniCover.UnitTests.Instrumentation.For/Class",
                    FullName = "System.Int32 MiniCover.UnitTests.Instrumentation.For/Class::Method(System.Int32)",
                    Name = "Method"
                },
                StartColumn = 17,
                StartLine = 16
            }
        };
    }
}
