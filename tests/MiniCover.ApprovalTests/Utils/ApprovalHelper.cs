using Shouldly;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace MiniCover.ApprovalTests.Utils
{
    class ApprovalHelper
    {
        public static void VerifyText(string content, string name, string extension, [CallerFilePath] string filepath = null, [CallerMemberName] string membername = null)
        {
            var resultDirectory = Path.Combine(Path.GetDirectoryName(filepath), "Result");
            var testFileName = Path.GetFileNameWithoutExtension(filepath);
            var outputDir = Path.Combine(resultDirectory, $"{testFileName}.{membername}");

            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            var approvedFile = Path.Combine(outputDir, $"{name}.approved.{extension}");
            var receivedFile = Path.Combine(outputDir, $"{name}.received.{extension}");

            File.WriteAllText(receivedFile, content);

            if (!File.Exists(approvedFile))
                throw new Exception($"File {approvedFile} doesn't exist");

            var approvedContent = File.ReadAllText(approvedFile);

            NormalizeLineBreaks(content).ShouldBe(NormalizeLineBreaks(approvedContent));
        }

        public static void VerifyByte(byte[] content, string name, string extension, [CallerFilePath] string filepath = null, [CallerMemberName] string membername = null)
        {
            var resultDirectory = Path.Combine(Path.GetDirectoryName(filepath), "Result");
            var testFileName = Path.GetFileNameWithoutExtension(filepath);
            var outputDir = Path.Combine(resultDirectory, $"{testFileName}.{membername}");

            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            var approvedFile = Path.Combine(outputDir, $"{name}.approved.{extension}");
            var receivedFile = Path.Combine(outputDir, $"{name}.received.{extension}");

            File.WriteAllBytes(receivedFile, content);

            if (!File.Exists(approvedFile))
                throw new Exception($"File {approvedFile} doesn't exist");

            var approvedContent = File.ReadAllBytes(approvedFile);

            if (!content.SequenceEqual(approvedContent))
                throw new Exception($"File {receivedFile} should have same content as file {approvedFile}");
        }

        private static string NormalizeLineBreaks(string content)
        {
            return content.Replace("\r\n", "\n");
        }
    }
}