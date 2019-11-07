using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MiniCover.Model;

namespace MiniCover.UnitTests.Instrumentation
{
    public class Lambda : BaseInstrumentationTest
    {
        public class Class
        {
            public int Method(params int[] values)
            {
                return values.Select(a => a * 2).Sum();
            }
        }

        public Lambda() : base(typeof(Class))
        {
        }

        public override void FunctionalTest()
        {
            new Class().Method(1, 2, 3).Should().Be(12);
        }

        public override string ExpectedIL => @".class Class
{
    .method public System.Int32 Method
    {
        .locals init (System.Int32 V_0, MiniCover.HitServices.HitService/MethodContext V_1, System.Int32 V_2)
        IL_0000: ldstr ""/tmp""
        IL_0005: ldstr ""MiniCover.UnitTests""
        IL_000a: ldstr ""MiniCover.UnitTests.Instrumentation.Lambda/Class""
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
            IL_0023: ldarg.1
            IL_0024: ldsfld System.Func`2<System.Int32,System.Int32> MiniCover.UnitTests.Instrumentation.Lambda/Class/<>c::<>9__0_0
            IL_0029: dup
            IL_002a: brtrue.s IL_0043
            IL_002c: pop
            IL_002d: ldsfld MiniCover.UnitTests.Instrumentation.Lambda/Class/<>c MiniCover.UnitTests.Instrumentation.Lambda/Class/<>c::<>9
            IL_0032: ldftn System.Int32 MiniCover.UnitTests.Instrumentation.Lambda/Class/<>c::<Method>b__0_0(System.Int32)
            IL_0038: newobj System.Void System.Func`2<System.Int32,System.Int32>::.ctor(System.Object,System.IntPtr)
            IL_003d: dup
            IL_003e: stsfld System.Func`2<System.Int32,System.Int32> MiniCover.UnitTests.Instrumentation.Lambda/Class/<>c::<>9__0_0
            IL_0043: call System.Collections.Generic.IEnumerable`1<!!1> System.Linq.Enumerable::Select<System.Int32,System.Int32>(System.Collections.Generic.IEnumerable`1<!!0>,System.Func`2<!!0,!!1>)
            IL_0048: call System.Int32 System.Linq.Enumerable::Sum(System.Collections.Generic.IEnumerable`1<System.Int32>)
            IL_004d: stloc.0
            IL_004e: br.s IL_0050
            IL_0050: ldloc.0
            IL_0051: stloc.2
            IL_0052: leave.s IL_005c
        }
        finally
        {
            IL_0054: nop
            IL_0055: ldloc.1
            IL_0056: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Dispose()
            IL_005b: endfinally
        }
        IL_005c: ldloc.2
        IL_005d: ret
    }
    .method public System.Void .ctor
    {
        .locals ()
        IL_0000: ldarg.0 // this
        IL_0001: call System.Void System.Object::.ctor()
        IL_0006: nop
        IL_0007: ret
    }
    .class <>c
    {
        .method static System.Void .cctor
        {
            .locals ()
            IL_0000: newobj System.Void MiniCover.UnitTests.Instrumentation.Lambda/Class/<>c::.ctor()
            IL_0005: stsfld MiniCover.UnitTests.Instrumentation.Lambda/Class/<>c MiniCover.UnitTests.Instrumentation.Lambda/Class/<>c::<>9
            IL_000a: ret
        }
        .method public System.Void .ctor
        {
            .locals ()
            IL_0000: ldarg.0 // this
            IL_0001: call System.Void System.Object::.ctor()
            IL_0006: nop
            IL_0007: ret
        }
        .method System.Int32 <Method>b__0_0
        {
            .locals init (MiniCover.HitServices.HitService/MethodContext V_0, System.Int32 V_1)
            IL_0000: ldstr ""/tmp""
            IL_0005: ldstr ""MiniCover.UnitTests""
            IL_000a: ldstr ""MiniCover.UnitTests.Instrumentation.Lambda/Class""
            IL_000f: ldstr ""Method""
            IL_0014: call MiniCover.HitServices.HitService/MethodContext MiniCover.HitServices.HitService::EnterMethod(System.String,System.String,System.String,System.String)
            IL_0019: stloc.0
            IL_001a: nop
            .try
            {
                IL_001b: ldloc.0
                IL_001c: ldc.i4.2
                IL_001d: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::HitInstruction(System.Int32)
                IL_0022: ldarg.1
                IL_0023: ldc.i4.2
                IL_0024: mul
                IL_0025: stloc.1
                IL_0026: leave.s IL_0030
            }
            finally
            {
                IL_0028: nop
                IL_0029: ldloc.0
                IL_002a: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Dispose()
                IL_002f: endfinally
            }
            IL_0030: ldloc.1
            IL_0031: ret
        }
    }
}
";

        public override IDictionary<int, int> ExpectedHits => new Dictionary<int, int>
        {
            [1] = 1,
            [2] = 3
        };

        public override InstrumentedInstruction[] ExpectedInstructions => new InstrumentedInstruction[] {
            new InstrumentedInstruction
            {
                Code = "return values.Select(a => a * 2).Sum();",
                EndColumn = 56,
                EndLine = 14,
                Id = 1,
                Instruction = "IL_0001: ldarg values",
                Method = new InstrumentedMethod {
                    Class = "MiniCover.UnitTests.Instrumentation.Lambda/Class",
                    Name = "Method",
                    FullName = "System.Int32 MiniCover.UnitTests.Instrumentation.Lambda/Class::Method(System.Int32[])",
                },
                StartColumn = 17,
                StartLine = 14,
            },
            new InstrumentedInstruction
            {
                Code = "a * 2",
                EndColumn = 48,
                EndLine = 14,
                Id = 2,
                Instruction = "IL_0000: ldarg a",
                Method = new InstrumentedMethod {
                    Class = "MiniCover.UnitTests.Instrumentation.Lambda/Class",
                    Name = "Method",
                    FullName = "System.Int32 MiniCover.UnitTests.Instrumentation.Lambda/Class::Method(System.Int32[])",
                },
                StartColumn = 43,
                StartLine = 14
            }
        };
    }
}
