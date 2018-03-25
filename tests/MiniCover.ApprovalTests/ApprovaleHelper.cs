using System.IO;
using System.Runtime.CompilerServices;
using ApprovalTests.Approvers;
using ApprovalTests.Core;
using ApprovalTests.Reporters;
using ApprovalTests.Writers;

namespace MiniCover.ApprovalTests
{
    class ApprovaleHelper
    {
        class SaneNamer : IApprovalNamer
        {
            public string SourcePath { get; set; }
            public string Name { get; set; }
        }

        public static void VerifyXml(string text, [CallerFilePath] string filepath = null, [CallerMemberName] string membername = null)
        {
            var writer = WriterFactory.CreateTextWriter(text, ".xml");
            var filename = Path.GetFileNameWithoutExtension(filepath);
            var filedir = Path.Combine(Path.GetDirectoryName(filepath), "Result");
            var namer = new SaneNamer { Name = filename + "." + membername, SourcePath = filedir };
            var reporter = new MultiReporter(new TravisCIReporter(), new DiffReporter());
            Approver.Verify(new FileApprover(writer, namer, true), reporter);
        }

        public static void VerifyJson(string text, [CallerFilePath] string filepath = null, [CallerMemberName] string membername = null)
        {
            var writer = WriterFactory.CreateTextWriter(text, ".json");

            var filename = Path.GetFileNameWithoutExtension(filepath);
            var filedir = Path.Combine(Path.GetDirectoryName(filepath), "Result");
            var namer = new SaneNamer { Name = filename + "." + membername, SourcePath = filedir };
            var reporter = new MultiReporter(new TravisCIReporter(), new DiffReporter());
            Approver.Verify(new FileApprover(writer, namer, true), reporter);
        }

        public static void VerifyByte(byte[] bytes, [CallerFilePath] string filepath = null, [CallerMemberName] string membername = null)
        {
            var writer = new ApprovalBinaryWriter(bytes, ".bin");
            var filename = Path.GetFileNameWithoutExtension(filepath);
            var filedir = Path.Combine(Path.GetDirectoryName(filepath), "Result");
            var namer = new SaneNamer { Name = filename + "." + membername, SourcePath = filedir };
            var reporter = new MultiReporter(new TravisCIReporter(), new DiffReporter());
            Approver.Verify(new FileApprover(writer, namer, true), reporter);
        }
    }
}