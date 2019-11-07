using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MiniCover.Model;

namespace MiniCover.UnitTests.Instrumentation
{
    public class EnumerableMethod : BaseInstrumentationTest
    {
        public class Class
        {
            public IEnumerable<int> Method(int max)
            {
                for (var i = 0; i < max; i++)
                    yield return i;
            }
        }

        public EnumerableMethod() : base(typeof(Class))
        {
        }

        public override void FunctionalTest()
        {
            new Class().Method(10).ToArray().Should().BeEquivalentTo(new int[] {
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9
            });
        }

        public override string ExpectedIL => @".class Class
{
    .method public System.Collections.Generic.IEnumerable`1<System.Int32> Method
    {
        .locals ()
        IL_0000: ldc.i4.s -2
        IL_0002: newobj System.Void MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::.ctor(System.Int32)
        IL_0007: dup
        IL_0008: ldarg.0 // this
        IL_0009: stfld MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::<>4__this
        IL_000e: dup
        IL_000f: ldarg.1
        IL_0010: stfld System.Int32 MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::<>3__max
        IL_0015: ret
    }
    .method public System.Void .ctor
    {
        .locals ()
        IL_0000: ldarg.0 // this
        IL_0001: call System.Void System.Object::.ctor()
        IL_0006: nop
        IL_0007: ret
    }
    .class <Method>d__0
    {
        .method public System.Void .ctor
        {
            .locals ()
            IL_0000: ldarg.0 // this
            IL_0001: call System.Void System.Object::.ctor()
            IL_0006: nop
            IL_0007: ldarg.0 // this
            IL_0008: ldarg.1
            IL_0009: stfld System.Int32 MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::<>1__state
            IL_000e: ldarg.0 // this
            IL_000f: call System.Int32 System.Environment::get_CurrentManagedThreadId()
            IL_0014: stfld System.Int32 MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::<>l__initialThreadId
            IL_0019: ret
        }
        .method System.Void System.IDisposable.Dispose
        {
            .locals ()
            IL_0000: ret
        }
        .method System.Boolean MoveNext
        {
            .locals init (System.Int32 V_0, System.Int32 V_1, System.Boolean V_2, MiniCover.HitServices.HitService/MethodContext V_3, System.Boolean V_4)
            IL_0000: ldstr ""/tmp""
            IL_0005: ldstr ""MiniCover.UnitTests""
            IL_000a: ldstr ""MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class""
            IL_000f: ldstr ""Method""
            IL_0014: call MiniCover.HitServices.HitService/MethodContext MiniCover.HitServices.HitService::EnterMethod(System.String,System.String,System.String,System.String)
            IL_0019: stloc.3
            IL_001a: nop
            .try
            {
                IL_001b: ldarg.0 // this
                IL_001c: ldfld System.Int32 MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::<>1__state
                IL_0021: stloc.0
                IL_0022: ldloc.0
                IL_0023: brfalse.s IL_002d
                IL_0025: br.s IL_0027
                IL_0027: ldloc.0
                IL_0028: ldc.i4.1
                IL_0029: beq.s IL_002f
                IL_002b: br.s IL_0031
                IL_002d: br.s IL_0039
                IL_002f: br.s IL_0070
                IL_0031: ldc.i4.0
                IL_0032: stloc.s V_4
                IL_0034: leave IL_00b4
                IL_0039: ldarg.0 // this
                IL_003a: ldc.i4.m1
                IL_003b: stfld System.Int32 MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::<>1__state
                IL_0040: nop
                IL_0041: ldloc.3
                IL_0042: ldc.i4.1
                IL_0043: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::HitInstruction(System.Int32)
                IL_0048: ldarg.0 // this
                IL_0049: ldc.i4.0
                IL_004a: stfld System.Int32 MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::<i>5__1
                IL_004f: br.s IL_008e
                IL_0051: ldloc.3
                IL_0052: ldc.i4.2
                IL_0053: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::HitInstruction(System.Int32)
                IL_0058: ldarg.0 // this
                IL_0059: ldarg.0 // this
                IL_005a: ldfld System.Int32 MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::<i>5__1
                IL_005f: stfld System.Int32 MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::<>2__current
                IL_0064: ldarg.0 // this
                IL_0065: ldc.i4.1
                IL_0066: stfld System.Int32 MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::<>1__state
                IL_006b: ldc.i4.1
                IL_006c: stloc.s V_4
                IL_006e: leave.s IL_00b4
                IL_0070: ldarg.0 // this
                IL_0071: ldc.i4.m1
                IL_0072: stfld System.Int32 MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::<>1__state
                IL_0077: ldloc.3
                IL_0078: ldc.i4.3
                IL_0079: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::HitInstruction(System.Int32)
                IL_007e: ldarg.0 // this
                IL_007f: ldfld System.Int32 MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::<i>5__1
                IL_0084: stloc.1
                IL_0085: ldarg.0 // this
                IL_0086: ldloc.1
                IL_0087: ldc.i4.1
                IL_0088: add
                IL_0089: stfld System.Int32 MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::<i>5__1
                IL_008e: ldloc.3
                IL_008f: ldc.i4.4
                IL_0090: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::HitInstruction(System.Int32)
                IL_0095: ldarg.0 // this
                IL_0096: ldfld System.Int32 MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::<i>5__1
                IL_009b: ldarg.0 // this
                IL_009c: ldfld System.Int32 MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::max
                IL_00a1: clt
                IL_00a3: stloc.2
                IL_00a4: ldloc.2
                IL_00a5: brtrue.s IL_0051
                IL_00a7: ldc.i4.0
                IL_00a8: stloc.s V_4
                IL_00aa: leave.s IL_00b4
            }
            finally
            {
                IL_00ac: nop
                IL_00ad: ldloc.3
                IL_00ae: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Dispose()
                IL_00b3: endfinally
            }
            IL_00b4: ldloc.s V_4
            IL_00b6: ret
        }
        .method System.Int32 System.Collections.Generic.IEnumerator<System.Int32>.get_Current
        {
            .locals ()
            IL_0000: ldarg.0 // this
            IL_0001: ldfld System.Int32 MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::<>2__current
            IL_0006: ret
        }
        .method System.Void System.Collections.IEnumerator.Reset
        {
            .locals ()
            IL_0000: newobj System.Void System.NotSupportedException::.ctor()
            IL_0005: throw
        }
        .method System.Object System.Collections.IEnumerator.get_Current
        {
            .locals ()
            IL_0000: ldarg.0 // this
            IL_0001: ldfld System.Int32 MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::<>2__current
            IL_0006: box System.Int32
            IL_000b: ret
        }
        .method System.Collections.Generic.IEnumerator`1<System.Int32> System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator
        {
            .locals init (MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0 V_0)
            IL_0000: ldarg.0 // this
            IL_0001: ldfld System.Int32 MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::<>1__state
            IL_0006: ldc.i4.s -2
            IL_0008: bne.un.s IL_0022
            IL_000a: ldarg.0 // this
            IL_000b: ldfld System.Int32 MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::<>l__initialThreadId
            IL_0010: call System.Int32 System.Environment::get_CurrentManagedThreadId()
            IL_0015: bne.un.s IL_0022
            IL_0017: ldarg.0 // this
            IL_0018: ldc.i4.0
            IL_0019: stfld System.Int32 MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::<>1__state
            IL_001e: ldarg.0 // this
            IL_001f: stloc.0
            IL_0020: br.s IL_0035
            IL_0022: ldc.i4.0
            IL_0023: newobj System.Void MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::.ctor(System.Int32)
            IL_0028: stloc.0
            IL_0029: ldloc.0
            IL_002a: ldarg.0 // this
            IL_002b: ldfld MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::<>4__this
            IL_0030: stfld MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::<>4__this
            IL_0035: ldloc.0
            IL_0036: ldarg.0 // this
            IL_0037: ldfld System.Int32 MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::<>3__max
            IL_003c: stfld System.Int32 MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::max
            IL_0041: ldloc.0
            IL_0042: ret
        }
        .method System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator
        {
            .locals ()
            IL_0000: ldarg.0 // this
            IL_0001: call System.Collections.Generic.IEnumerator`1<System.Int32> MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class/<Method>d__0::System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator()
            IL_0006: ret
        }
    }
}
";

        public override IDictionary<int, int> ExpectedHits => new Dictionary<int, int>
        {
            [1] = 1,
            [2] = 10,
            [3] = 10,
            [4] = 11
        };

        public override InstrumentedInstruction[] ExpectedInstructions => new InstrumentedInstruction[]
        {
            new InstrumentedInstruction
            {
                Code = "var i = 0",
                EndColumn = 31,
                EndLine = 14,
                Id = 1,
                Instruction = "IL_0020: ldarg ",
                Method = new InstrumentedMethod {
                    Class = "MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class",
                    Name = "Method",
                    FullName = "System.Collections.Generic.IEnumerable`1<System.Int32> MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class::Method(System.Int32)",
                },
                StartColumn = 22,
                StartLine = 14,
            },
            new InstrumentedInstruction
            {
                Code = "yield return i;",
                EndColumn = 36,
                EndLine = 15,
                Id = 2,
                Instruction = "IL_0029: ldarg ",
                Method = new InstrumentedMethod {
                    Class = "MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class",
                    Name = "Method",
                    FullName = "System.Collections.Generic.IEnumerable`1<System.Int32> MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class::Method(System.Int32)",
                },
                StartColumn = 21,
                StartLine = 15,
            },
            new InstrumentedInstruction
            {
                Code = "i++",
                EndColumn = 45,
                EndLine = 14,
                Id = 3,
                Instruction = "IL_0045: ldarg ",
                Method = new InstrumentedMethod {
                    Class = "MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class",
                    Name = "Method",
                    FullName = "System.Collections.Generic.IEnumerable`1<System.Int32> MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class::Method(System.Int32)",
                },
                StartColumn = 42,
                StartLine = 14,
            },
            new InstrumentedInstruction
            {
                Code = "i < max",
                EndColumn = 40,
                EndLine = 14,
                Id = 4,
                Instruction = "IL_0055: ldarg ",
                Method = new InstrumentedMethod {
                    Class = "MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class",
                    Name = "Method",
                    FullName = "System.Collections.Generic.IEnumerable`1<System.Int32> MiniCover.UnitTests.Instrumentation.EnumerableMethod/Class::Method(System.Int32)",
                },
                StartColumn = 33,
                StartLine = 14,
            }
        };
    }
}
