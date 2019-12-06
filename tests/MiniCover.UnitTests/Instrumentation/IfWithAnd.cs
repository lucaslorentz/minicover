using System.Collections.Generic;
using FluentAssertions;
using MiniCover.Model;

namespace MiniCover.UnitTests.Instrumentation
{
    public class IfWithAnd : BaseTest
    {
        public class Class
        {
            public bool Method(int x)
            {
                if (x % 2 == 0 && x % 3 == 0)
                    return true;

                return false;
            }
        }

        public IfWithAnd() : base(typeof(Class).GetMethod(nameof(Class.Method)))
        {
        }

        public override void FunctionalTest()
        {
            new Class().Method(2).Should().Be(false);
            new Class().Method(3).Should().Be(false);
        }

        public override string ExpectedIL => @".locals init (System.Boolean V_0, System.Boolean V_1, MiniCover.HitServices.HitService/MethodContext V_2, System.Boolean V_3)
IL_0000: ldstr ""/tmp""
IL_0005: ldstr ""MiniCover.UnitTests""
IL_000a: ldstr ""MiniCover.UnitTests.Instrumentation.IfWithAnd/Class""
IL_000f: ldstr ""Method""
IL_0014: call MiniCover.HitServices.HitService/MethodContext MiniCover.HitServices.HitService::EnterMethod(System.String,System.String,System.String,System.String)
IL_0019: stloc.2
IL_001a: nop
.try
{
    IL_001b: nop
    IL_001c: ldloc.2
    IL_001d: ldc.i4.1
    IL_001e: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Hit(System.Int32)
    IL_0023: ldarg.1
    IL_0024: ldc.i4.2
    IL_0025: rem
    IL_0026: brtrue.s IL_0037
    IL_0028: ldloc.2
    IL_0029: ldc.i4.4
    IL_002a: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Hit(System.Int32)
    IL_002f: ldarg.1
    IL_0030: ldc.i4.3
    IL_0031: rem
    IL_0032: ldc.i4.0
    IL_0033: ceq
    IL_0035: br.s IL_003f
    IL_0037: ldloc.2
    IL_0038: ldc.i4.5
    IL_0039: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Hit(System.Int32)
    IL_003e: ldc.i4.0
    IL_003f: stloc.0
    IL_0040: ldloc.0
    IL_0041: brfalse.s IL_004e
    IL_0043: ldloc.2
    IL_0044: ldc.i4.2
    IL_0045: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Hit(System.Int32)
    IL_004a: ldc.i4.1
    IL_004b: stloc.1
    IL_004c: br.s IL_0059
    IL_004e: ldloc.2
    IL_004f: ldc.i4.3
    IL_0050: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Hit(System.Int32)
    IL_0055: ldc.i4.0
    IL_0056: stloc.1
    IL_0057: br.s IL_0059
    IL_0059: ldloc.1
    IL_005a: stloc.3
    IL_005b: leave.s IL_0065
}
finally
{
    IL_005d: nop
    IL_005e: ldloc.2
    IL_005f: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Dispose()
    IL_0064: endfinally
}
IL_0065: ldloc.3
IL_0066: ret
";

        public override IDictionary<int, int> ExpectedHits => new Dictionary<int, int>
        {
            [1] = 2,
            [4] = 1,
            [3] = 2,
            [5] = 1
        };

        public override InstrumentedSequence[] ExpectedInstructions => new InstrumentedSequence[]
        {
            new InstrumentedSequence
            {
                Code = "if (x % 2 == 0 && x % 3 == 0)",
                Conditions = new InstrumentedCondition[] {
                    new InstrumentedCondition
                    {
                        Instruction = "IL_000f: stloc V_0",
                        Branches = new InstrumentedBranch[]
                        {
                            new InstrumentedBranch
                            {
                                External = true,
                                HitId = 2,
                                Instruction = "IL_0013: ldc.i4 1"
                            },
                            new InstrumentedBranch
                            {
                                External = true,
                                HitId = 3,
                                Instruction = "IL_0017: ldc.i4 0"
                            }
                        }
                    },
                    new InstrumentedCondition
                    {
                        Instruction = "IL_0004: brtrue IL_0000",
                        Branches = new InstrumentedBranch[]
                        {
                            new InstrumentedBranch
                            {
                                External = false,
                                HitId = 4,
                                Instruction = "IL_0006: ldarg x"
                            },
                            new InstrumentedBranch
                            {
                                External = false,
                                HitId = 5,
                                Instruction = "IL_000e: ldc.i4 0"
                            }
                        }
                    }
                },
                EndColumn = 46,
                EndLine = 13,
                HitId = 1,
                Instruction = "IL_0001: ldarg x",
                Method = new InstrumentedMethod
                {
                    Class = "MiniCover.UnitTests.Instrumentation.IfWithAnd/Class",
                    FullName = "System.Boolean MiniCover.UnitTests.Instrumentation.IfWithAnd/Class::Method(System.Int32)",
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
                Instruction = "IL_0013: ldc.i4 1",
                Method = new InstrumentedMethod
                {
                    Class = "MiniCover.UnitTests.Instrumentation.IfWithAnd/Class",
                    FullName = "System.Boolean MiniCover.UnitTests.Instrumentation.IfWithAnd/Class::Method(System.Int32)",
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
                Instruction = "IL_0017: ldc.i4 0",
                Method = new InstrumentedMethod
                {
                    Class = "MiniCover.UnitTests.Instrumentation.IfWithAnd/Class",
                    FullName = "System.Boolean MiniCover.UnitTests.Instrumentation.IfWithAnd/Class::Method(System.Int32)",
                    Name = "Method"
                },
                StartColumn = 17,
                StartLine = 16
            }
        };
    }
}
