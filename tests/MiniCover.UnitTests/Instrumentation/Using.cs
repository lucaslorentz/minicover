using System.Collections.Generic;
using System.IO;
using FluentAssertions;

namespace MiniCover.UnitTests.Instrumentation
{
    public class Using : BaseTest
    {
        public class Class
        {
            public byte[] Method()
            {
                using (var stream = new MemoryStream())
                {
                    stream.WriteByte(0x00);
                    return stream.ToArray();
                }
            }
        }

        public Using() : base(typeof(Class).GetMethod(nameof(Class.Method)))
        {
        }

        public override void FunctionalTest()
        {
            new Class().Method().Should().BeEquivalentTo(new byte[] { 0x00 });
        }

        public override string ExpectedIL => @".locals init (System.IO.MemoryStream stream, System.Byte[] V_1, MiniCover.HitServices.HitService/MethodContext V_2, System.Byte[] V_3)
IL_0000: ldstr ""/tmp""
IL_0005: ldstr ""MiniCover.UnitTests""
IL_000a: ldstr ""MiniCover.UnitTests.Instrumentation.Using/Class""
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
    IL_0023: newobj System.Void System.IO.MemoryStream::.ctor()
    IL_0028: stloc.0
    .try
    {
        IL_0029: nop
        IL_002a: ldloc.2
        IL_002b: ldc.i4.2
        IL_002c: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Hit(System.Int32)
        IL_0031: ldloc.0
        IL_0032: ldc.i4.0
        IL_0033: callvirt System.Void System.IO.Stream::WriteByte(System.Byte)
        IL_0038: nop
        IL_0039: ldloc.2
        IL_003a: ldc.i4.3
        IL_003b: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Hit(System.Int32)
        IL_0040: ldloc.0
        IL_0041: callvirt System.Byte[] System.IO.MemoryStream::ToArray()
        IL_0046: stloc.1
        IL_0047: leave.s IL_0054
    }
    finally
    {
        IL_0049: ldloc.0
        IL_004a: brfalse.s IL_0053
        IL_004c: ldloc.0
        IL_004d: callvirt System.Void System.IDisposable::Dispose()
        IL_0052: nop
        IL_0053: endfinally
    }
    IL_0054: ldloc.1
    IL_0055: stloc.3
    IL_0056: leave.s IL_0060
}
finally
{
    IL_0058: nop
    IL_0059: ldloc.2
    IL_005a: callvirt System.Void MiniCover.HitServices.HitService/MethodContext::Dispose()
    IL_005f: endfinally
}
IL_0060: ldloc.3
IL_0061: ret
";

        public override IDictionary<int, int> ExpectedHits => new Dictionary<int, int>
        {
            [1] = 1,
            [2] = 1,
            [3] = 1
        };
    }
}
