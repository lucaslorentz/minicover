using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using MiniCover.Model;

namespace MiniCover.UnitTests.Instrumentation
{
    public class AsyncMethod : BaseInstrumentationTest
    {
        public class Class
        {
            public async Task<int> Method()
            {
                var x = 0;
                await Task.Delay(5);
                x++;
                await Task.Delay(5);
                x++;
                return x;
            }
        }

        public AsyncMethod() : base(typeof(Class))
        {
        }

        public override void FunctionalTest()
        {
            new Class().Method().Result.Should().Be(2);
        }

        public override string ExpectedIL => @".class Class
{
    .method public System.Threading.Tasks.Task`1<System.Int32> Method
    {
        .locals init (MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0 V_0, System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<System.Int32> V_1)
        IL_0000: newobj System.Void MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::.ctor()
        IL_0005: stloc.0
        IL_0006: ldloc.0
        IL_0007: ldarg.0 // this
        IL_0008: stfld MiniCover.UnitTests.Instrumentation.AsyncMethod/Class MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<>4__this
        IL_000d: ldloc.0
        IL_000e: call System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<System.Int32>::Create()
        IL_0013: stfld System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<System.Int32> MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<>t__builder
        IL_0018: ldloc.0
        IL_0019: ldc.i4.m1
        IL_001a: stfld System.Int32 MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<>1__state
        IL_001f: ldloc.0
        IL_0020: ldfld System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<System.Int32> MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<>t__builder
        IL_0025: stloc.1
        IL_0026: ldloca.s V_1
        IL_0028: ldloca.s V_0
        IL_002a: call System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<System.Int32>::Start<MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0>(!!0&)
        IL_002f: ldloc.0
        IL_0030: ldflda System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<System.Int32> MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<>t__builder
        IL_0035: call System.Threading.Tasks.Task`1<!0> System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<System.Int32>::get_Task()
        IL_003a: ret
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
            IL_0007: ret
        }
        .method System.Void MoveNext
        {
            .locals init (System.Int32 V_0, System.Int32 V_1, System.Runtime.CompilerServices.TaskAwaiter V_2, MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0 V_3, System.Int32 V_4, System.Runtime.CompilerServices.TaskAwaiter V_5, System.Exception V_6, MiniCover.HitServices.HitService/MethodContext V_7)
            IL_0000: ldstr ""/tmp""
            IL_0005: ldstr ""MiniCover.UnitTests""
            IL_000a: ldstr ""MiniCover.UnitTests.Instrumentation.AsyncMethod/Class""
            IL_000f: ldstr ""Method""
            IL_0014: call MiniCover.HitServices.HitService/MethodContext MiniCover.HitServices.HitService::EnterMethod(System.String,System.String,System.String,System.String)
            IL_0019: stloc.s V_7
            IL_001b: nop
            .try
            {
                IL_001c: ldarg.0 // this
                IL_001d: ldfld System.Int32 MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<>1__state
                IL_0022: stloc.0
                .try
                {
                    IL_0023: ldloc.0
                    IL_0024: brfalse.s IL_002e
                    IL_0026: br.s IL_0028
                    IL_0028: ldloc.0
                    IL_0029: ldc.i4.1
                    IL_002a: beq.s IL_0030
                    IL_002c: br.s IL_0035
                    IL_002e: br.s IL_0089
                    IL_0030: br IL_010d
                    IL_0035: nop
                    IL_0036: ldloc.s V_7
                    IL_0038: ldc.i4.1
                    IL_0039: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::HitInstruction(System.Int32)
                    IL_003e: ldarg.0 // this
                    IL_003f: ldc.i4.0
                    IL_0040: stfld System.Int32 MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<x>5__1
                    IL_0045: ldloc.s V_7
                    IL_0047: ldc.i4.2
                    IL_0048: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::HitInstruction(System.Int32)
                    IL_004d: ldc.i4.5
                    IL_004e: call System.Threading.Tasks.Task System.Threading.Tasks.Task::Delay(System.Int32)
                    IL_0053: callvirt System.Runtime.CompilerServices.TaskAwaiter System.Threading.Tasks.Task::GetAwaiter()
                    IL_0058: stloc.2
                    IL_0059: ldloca.s V_2
                    IL_005b: call System.Boolean System.Runtime.CompilerServices.TaskAwaiter::get_IsCompleted()
                    IL_0060: brtrue.s IL_00a5
                    IL_0062: ldarg.0 // this
                    IL_0063: ldc.i4.0
                    IL_0064: dup
                    IL_0065: stloc.0
                    IL_0066: stfld System.Int32 MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<>1__state
                    IL_006b: ldarg.0 // this
                    IL_006c: ldloc.2
                    IL_006d: stfld System.Runtime.CompilerServices.TaskAwaiter MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<>u__1
                    IL_0072: ldarg.0 // this
                    IL_0073: stloc.3
                    IL_0074: ldarg.0 // this
                    IL_0075: ldflda System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<System.Int32> MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<>t__builder
                    IL_007a: ldloca.s V_2
                    IL_007c: ldloca.s V_3
                    IL_007e: call System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<System.Int32>::AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0>(!!0&,!!1&)
                    IL_0083: nop
                    IL_0084: leave IL_018c
                    IL_0089: ldarg.0 // this
                    IL_008a: ldfld System.Runtime.CompilerServices.TaskAwaiter MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<>u__1
                    IL_008f: stloc.2
                    IL_0090: ldarg.0 // this
                    IL_0091: ldflda System.Runtime.CompilerServices.TaskAwaiter MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<>u__1
                    IL_0096: initobj System.Runtime.CompilerServices.TaskAwaiter
                    IL_009c: ldarg.0 // this
                    IL_009d: ldc.i4.m1
                    IL_009e: dup
                    IL_009f: stloc.0
                    IL_00a0: stfld System.Int32 MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<>1__state
                    IL_00a5: ldloca.s V_2
                    IL_00a7: call System.Void System.Runtime.CompilerServices.TaskAwaiter::GetResult()
                    IL_00ac: nop
                    IL_00ad: ldloc.s V_7
                    IL_00af: ldc.i4.3
                    IL_00b0: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::HitInstruction(System.Int32)
                    IL_00b5: ldarg.0 // this
                    IL_00b6: ldfld System.Int32 MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<x>5__1
                    IL_00bb: stloc.s V_4
                    IL_00bd: ldarg.0 // this
                    IL_00be: ldloc.s V_4
                    IL_00c0: ldc.i4.1
                    IL_00c1: add
                    IL_00c2: stfld System.Int32 MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<x>5__1
                    IL_00c7: ldloc.s V_7
                    IL_00c9: ldc.i4.4
                    IL_00ca: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::HitInstruction(System.Int32)
                    IL_00cf: ldc.i4.5
                    IL_00d0: call System.Threading.Tasks.Task System.Threading.Tasks.Task::Delay(System.Int32)
                    IL_00d5: callvirt System.Runtime.CompilerServices.TaskAwaiter System.Threading.Tasks.Task::GetAwaiter()
                    IL_00da: stloc.s V_5
                    IL_00dc: ldloca.s V_5
                    IL_00de: call System.Boolean System.Runtime.CompilerServices.TaskAwaiter::get_IsCompleted()
                    IL_00e3: brtrue.s IL_012a
                    IL_00e5: ldarg.0 // this
                    IL_00e6: ldc.i4.1
                    IL_00e7: dup
                    IL_00e8: stloc.0
                    IL_00e9: stfld System.Int32 MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<>1__state
                    IL_00ee: ldarg.0 // this
                    IL_00ef: ldloc.s V_5
                    IL_00f1: stfld System.Runtime.CompilerServices.TaskAwaiter MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<>u__1
                    IL_00f6: ldarg.0 // this
                    IL_00f7: stloc.3
                    IL_00f8: ldarg.0 // this
                    IL_00f9: ldflda System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<System.Int32> MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<>t__builder
                    IL_00fe: ldloca.s V_5
                    IL_0100: ldloca.s V_3
                    IL_0102: call System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<System.Int32>::AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0>(!!0&,!!1&)
                    IL_0107: nop
                    IL_0108: leave IL_018c
                    IL_010d: ldarg.0 // this
                    IL_010e: ldfld System.Runtime.CompilerServices.TaskAwaiter MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<>u__1
                    IL_0113: stloc.s V_5
                    IL_0115: ldarg.0 // this
                    IL_0116: ldflda System.Runtime.CompilerServices.TaskAwaiter MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<>u__1
                    IL_011b: initobj System.Runtime.CompilerServices.TaskAwaiter
                    IL_0121: ldarg.0 // this
                    IL_0122: ldc.i4.m1
                    IL_0123: dup
                    IL_0124: stloc.0
                    IL_0125: stfld System.Int32 MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<>1__state
                    IL_012a: ldloca.s V_5
                    IL_012c: call System.Void System.Runtime.CompilerServices.TaskAwaiter::GetResult()
                    IL_0131: nop
                    IL_0132: ldloc.s V_7
                    IL_0134: ldc.i4.5
                    IL_0135: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::HitInstruction(System.Int32)
                    IL_013a: ldarg.0 // this
                    IL_013b: ldfld System.Int32 MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<x>5__1
                    IL_0140: stloc.s V_4
                    IL_0142: ldarg.0 // this
                    IL_0143: ldloc.s V_4
                    IL_0145: ldc.i4.1
                    IL_0146: add
                    IL_0147: stfld System.Int32 MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<x>5__1
                    IL_014c: ldloc.s V_7
                    IL_014e: ldc.i4.6
                    IL_014f: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::HitInstruction(System.Int32)
                    IL_0154: ldarg.0 // this
                    IL_0155: ldfld System.Int32 MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<x>5__1
                    IL_015a: stloc.1
                    IL_015b: leave.s IL_0177
                }
                catch System.Exception
                {
                    IL_015d: stloc.s V_6
                    IL_015f: ldarg.0 // this
                    IL_0160: ldc.i4.s -2
                    IL_0162: stfld System.Int32 MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<>1__state
                    IL_0167: ldarg.0 // this
                    IL_0168: ldflda System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<System.Int32> MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<>t__builder
                    IL_016d: ldloc.s V_6
                    IL_016f: call System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<System.Int32>::SetException(System.Exception)
                    IL_0174: nop
                    IL_0175: leave.s IL_018c
                }
                IL_0177: ldarg.0 // this
                IL_0178: ldc.i4.s -2
                IL_017a: stfld System.Int32 MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<>1__state
                IL_017f: ldarg.0 // this
                IL_0180: ldflda System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<System.Int32> MiniCover.UnitTests.Instrumentation.AsyncMethod/Class/<Method>d__0::<>t__builder
                IL_0185: ldloc.1
                IL_0186: call System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<System.Int32>::SetResult(!0)
                IL_018b: nop
                IL_018c: leave.s IL_0197
            }
            finally
            {
                IL_018e: nop
                IL_018f: ldloc.s V_7
                IL_0191: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Dispose()
                IL_0196: endfinally
            }
            IL_0197: ret
        }
        .method System.Void SetStateMachine
        {
            .locals ()
            IL_0000: ret
        }
    }
}
";

        public override IDictionary<int, int> ExpectedHits => new Dictionary<int, int>
        {
            [1] = 1,
            [2] = 1,
            [3] = 1,
            [4] = 1,
            [5] = 1,
            [6] = 1
        };

        public override InstrumentedInstruction[] ExpectedInstructions => new InstrumentedInstruction[]
        {
            new InstrumentedInstruction
            {
                Code = "var x = 0;",
                EndColumn = 27,
                EndLine = 14,
                Id = 1,
                Instruction = "IL_001a: ldarg ",
                Method = new InstrumentedMethod {
                    Class = "MiniCover.UnitTests.Instrumentation.AsyncMethod/Class",
                    Name = "Method",
                    FullName = "System.Threading.Tasks.Task`1<System.Int32> MiniCover.UnitTests.Instrumentation.AsyncMethod/Class::Method()",
                },
                StartColumn = 17,
                StartLine = 14
            },
            new InstrumentedInstruction
            {
                Code = "await Task.Delay(5);",
                EndColumn = 37,
                EndLine = 15,
                Id = 2,
                Instruction = "IL_0021: ldc.i4 5",
                Method = new InstrumentedMethod {
                    Class = "MiniCover.UnitTests.Instrumentation.AsyncMethod/Class",
                    Name = "Method",
                    FullName = "System.Threading.Tasks.Task`1<System.Int32> MiniCover.UnitTests.Instrumentation.AsyncMethod/Class::Method()",
                },
                StartColumn = 17,
                StartLine = 15
            },
            new InstrumentedInstruction
            {
                Code = "x++;",
                EndColumn = 21,
                EndLine = 16,
                Id = 3,
                Instruction = "IL_0081: ldarg ",
                Method = new InstrumentedMethod {
                    Class = "MiniCover.UnitTests.Instrumentation.AsyncMethod/Class",
                    Name = "Method",
                    FullName = "System.Threading.Tasks.Task`1<System.Int32> MiniCover.UnitTests.Instrumentation.AsyncMethod/Class::Method()",
                },
                StartColumn = 17,
                StartLine = 16
            },
            new InstrumentedInstruction
            {
                Code = "await Task.Delay(5);",
                EndColumn = 37,
                EndLine = 17,
                Id = 4,
                Instruction = "IL_0093: ldc.i4 5",
                Method = new InstrumentedMethod {
                    Class = "MiniCover.UnitTests.Instrumentation.AsyncMethod/Class",
                    Name = "Method",
                    FullName = "System.Threading.Tasks.Task`1<System.Int32> MiniCover.UnitTests.Instrumentation.AsyncMethod/Class::Method()",
                },
                StartColumn = 17,
                StartLine = 17,
            },
            new InstrumentedInstruction
            {
                Code = "x++;",
                EndColumn = 21,
                EndLine = 18,
                Id = 5,
                Instruction = "IL_00f3: ldarg ",
                Method = new InstrumentedMethod {
                    Class = "MiniCover.UnitTests.Instrumentation.AsyncMethod/Class",
                    Name = "Method",
                    FullName = "System.Threading.Tasks.Task`1<System.Int32> MiniCover.UnitTests.Instrumentation.AsyncMethod/Class::Method()",
                },
                StartColumn = 17,
                StartLine = 18
            },
            new InstrumentedInstruction
            {
                Code = "return x;",
                EndColumn = 26,
                EndLine = 19,
                Id = 6,
                Instruction = "IL_0105: ldarg ",
                Method = new InstrumentedMethod {
                    Class = "MiniCover.UnitTests.Instrumentation.AsyncMethod/Class",
                    Name = "Method",
                    FullName = "System.Threading.Tasks.Task`1<System.Int32> MiniCover.UnitTests.Instrumentation.AsyncMethod/Class::Method()",
                },
                StartColumn = 17,
                StartLine = 19
            }
        };
    }
}
