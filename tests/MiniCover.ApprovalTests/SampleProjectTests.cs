using Microsoft.DotNet.Cli.Utils;
using MiniCover.ApprovalTests.Utils;
using Shouldly;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace MiniCover.ApprovalTests
{
    public class SampleProjectTests
    {
        [Fact]
        public void Execute()
        {
            var rootDirectory = SolutionDir.GetRootPath();
            var sampleDirectory = Path.Combine(rootDirectory, "sample");
            var miniCoverPath = Path.Combine(rootDirectory, "src/MiniCover");
            var toolPath = Path.Combine(sampleDirectory, "tools");
            var minicoverWorkdir = Path.Combine(sampleDirectory, "coverage");

            if (Directory.Exists(minicoverWorkdir))
                Directory.Delete(minicoverWorkdir, true);

            var result = Command.CreateDotNet("restore", new[] { "--no-cache" })
                .WorkingDirectory(sampleDirectory)
                .CaptureStdOut()
                .CaptureStdErr()
                .Execute();
            result.ExitCode.ShouldBe(0, $"output:{result.StdOut}{Environment.NewLine}errors:{result.StdErr}");
            result.StdErr.ShouldBeNullOrEmpty();

            result = Command.CreateDotNet("clean", new string[0])
                .WorkingDirectory(sampleDirectory)
                .CaptureStdOut()
                .CaptureStdErr()
                .Execute();
            result.ExitCode.ShouldBe(0, $"output:{result.StdOut}{Environment.NewLine}errors:{result.StdErr}");
            result.StdErr.ShouldBeNullOrEmpty();

            result = Command.CreateDotNet("build", new[] { "--no-restore" })
                .WorkingDirectory(sampleDirectory)
                .CaptureStdOut()
                .CaptureStdErr()
                .Execute();
            result.ExitCode.ShouldBe(0, $"output:{result.StdOut}{Environment.NewLine}errors:{result.StdErr}");
            result.StdErr.ShouldBeNullOrEmpty();

            result = Command.CreateDotNet("run", new[] { "--no-build", "--", "instrument", "--workdir", minicoverWorkdir, "--parentdir", "../", "--assemblies", "**/bin/**/*.dll", "--sources", "**/*.cs" })
                .WorkingDirectory(miniCoverPath)
                .CaptureStdOut()
                .CaptureStdErr()
                .Execute();
            result.ExitCode.ShouldBe(0, $"output:{result.StdOut}{Environment.NewLine}errors:{result.StdErr}");
            result.StdErr.ShouldBeNullOrEmpty();

            result = Command.CreateDotNet("run", new[] { "--no-build", "--", "reset", "--workdir", minicoverWorkdir })
                .WorkingDirectory(miniCoverPath)
                .CaptureStdOut()
                .CaptureStdErr()
                .Execute();
            result.ExitCode.ShouldBe(0, $"output:{result.StdOut}{Environment.NewLine}errors:{result.StdErr}");
            result.StdErr.ShouldBeNullOrEmpty();

            var testProjects = Directory.EnumerateFiles(Path.Combine(sampleDirectory, "test"), "*.csproj",
                SearchOption.AllDirectories);
            foreach (var testProject in testProjects.OrderBy(a => new FileInfo(a).Name))
            {
                result = Command.CreateDotNet("test", new[] { testProject, "--no-build" })
                    .WorkingDirectory(sampleDirectory)
                    .CaptureStdOut()
                    .CaptureStdErr()
                    .Execute();
                result.ExitCode.ShouldBe(0, $"output:{result.StdOut}{Environment.NewLine}errors:{result.StdErr}");
                result.StdErr.ShouldBeNullOrEmpty();
            }

            result = Command.CreateDotNet("run", new[] { "--no-build", "--", "uninstrument", "--workdir", minicoverWorkdir })
                .WorkingDirectory(miniCoverPath)
                .CaptureStdOut()
                .CaptureStdErr()
                .Execute();
            result.ExitCode.ShouldBe(0, $"output:{result.StdOut}{Environment.NewLine}errors:{result.StdErr}");
            result.StdErr.ShouldBeNullOrEmpty();

            var coverageJson = StringConverger.ApplyCleanup(File.ReadAllText(Path.Combine(minicoverWorkdir, "coverage.json")), new Uri(sampleDirectory).LocalPath);
            ApprovalHelper.VerifyText(coverageJson, "coverage", "json");

            var coverageHits = File.ReadAllBytes(Path.Combine(minicoverWorkdir, "coverage.hits"));
            ApprovalHelper.VerifyByte(coverageHits, "coverage", "hits");

            result = Command.CreateDotNet("run", new[] { "--no-build", "--", "report", "--workdir", minicoverWorkdir, "--threshold", "80" })
                .WorkingDirectory(miniCoverPath)
                .CaptureStdOut()
                .CaptureStdErr()
                .Execute();
            result.ExitCode.ShouldBe(0, $"output:{result.StdOut}{Environment.NewLine}errors:{result.StdErr}");
            result.StdErr.ShouldBeNullOrEmpty();

            result = Command.CreateDotNet("run", new[] { "--no-build", "--", "htmlreport", "--workdir", minicoverWorkdir, "--threshold", "80" })
                .WorkingDirectory(miniCoverPath)
                .CaptureStdOut()
                .CaptureStdErr()
                .Execute();
            result.ExitCode.ShouldBe(0, $"output:{result.StdOut}{Environment.NewLine}errors:{result.StdErr}");
            result.StdErr.ShouldBeNullOrEmpty();

            result = Command.CreateDotNet("run", new[] { "--no-build", "--", "xmlreport", "--workdir", minicoverWorkdir, "--threshold", "80" })
                .WorkingDirectory(miniCoverPath)
                .CaptureStdOut()
                .CaptureStdErr()
                .Execute();
            result.ExitCode.ShouldBe(0, $"output:{result.StdOut}{Environment.NewLine}errors:{result.StdErr}");
            result.StdErr.ShouldBeNullOrEmpty();

            result = Command.CreateDotNet("run", new[] { "--no-build", "--", "opencoverreport", "--workdir", minicoverWorkdir, "--threshold", "80" })
                .WorkingDirectory(miniCoverPath)
                .CaptureStdOut()
                .CaptureStdErr()
                .Execute();
            result.ExitCode.ShouldBe(0, $"output:{result.StdOut}{Environment.NewLine}errors:{result.StdErr}");
            result.StdErr.ShouldBeNullOrEmpty();

            result = Command.CreateDotNet("run", new[] { "--no-build", "--", "cloverreport", "--workdir", minicoverWorkdir, "--threshold", "80" })
                .WorkingDirectory(miniCoverPath)
                .CaptureStdOut()
                .CaptureStdErr()
                .Execute();
            result.ExitCode.ShouldBe(0, $"output:{result.StdOut}{Environment.NewLine}errors:{result.StdErr}");
            result.StdErr.ShouldBeNullOrEmpty();
        }
    }
}