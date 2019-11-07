using System;
using System.Collections.Generic;
using FluentAssertions;

namespace MiniCover.UnitTests.Instrumentation
{
    public class MethodThrowsException : BaseInstrumentationTest
    {
        public class Class
        {
            public int Method(int value)
            {
                value = value * 2;
                throw new Exception("Some exception");
            }
        }

        public MethodThrowsException() : base(typeof(Class).GetMethod(nameof(Class.Method)))
        {
        }

        public override void FunctionalTest()
        {
            try
            {
                new Class().Method(5).Should().Be(10);
            }
            catch
            {
            }
        }

        public override string ExpectedIL => @".locals init (MiniCover.HitServices.HitService/MethodContext V_0, System.Int32 V_1)
IL_0000: ldstr ""/tmp""
IL_0005: ldstr ""MiniCover.UnitTests""
IL_000a: ldstr ""MiniCover.UnitTests.Instrumentation.MethodThrowsException/Class""
IL_000f: ldstr ""Method""
IL_0014: call MiniCover.HitServices.HitService/MethodContext MiniCover.HitServices.HitService::EnterMethod(System.String,System.String,System.String,System.String)
IL_0019: stloc.0
IL_001a: nop
.try
{
    IL_001b: nop
    IL_001c: ldloc.0
    IL_001d: ldc.i4.1
    IL_001e: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::HitInstruction(System.Int32)
    IL_0023: ldarg.1
    IL_0024: ldc.i4.2
    IL_0025: mul
    IL_0026: starg 1
    IL_002a: ldloc.0
    IL_002b: ldc.i4.2
    IL_002c: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::HitInstruction(System.Int32)
    IL_0031: ldstr ""Some exception""
    IL_0036: newobj System.Void System.Exception::.ctor(System.String)
    IL_003b: throw
}
finally
{
    IL_003c: nop
    IL_003d: ldloc.0
    IL_003e: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Dispose()
    IL_0043: endfinally
}
IL_0044: ldloc.1
IL_0045: ret
";

        public override IDictionary<int, int> ExpectedHits => new Dictionary<int, int>
        {
            [1] = 1,
            [2] = 1
        };
    }
}
