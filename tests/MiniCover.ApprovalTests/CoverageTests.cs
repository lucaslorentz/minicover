using System;
using System.IO;
using System.Linq;
using Microsoft.DotNet.Cli.Utils;
using Shouldly;
using Xunit;

namespace MiniCover.ApprovalTests
{
    public class CoverageTests
    {
        [Fact]
        public void Execute()
        {
            var sampleRootDirectory = Path.Combine(new SolutionDir().GetRootPath(this.GetType()), "sample");
            var sampleSolution = Path.Combine(sampleRootDirectory, "Sample.sln");
            var toolPath = Path.Combine(sampleRootDirectory, "tools");
            var workdir = $"../coverage/test";
            var tempDirectory = Path.Combine(toolPath, workdir);
            if (Directory.Exists(tempDirectory))
                Directory.Delete(tempDirectory, true);
            var commandRestore = Command.CreateDotNet("restore", new[] {sampleSolution, "--no-cache"});
            var result = commandRestore.Execute();
            result.ExitCode.ShouldBe(0);

            var commandBuild = Command.CreateDotNet("build", new[] {sampleSolution, "--no-restore"});
            result = commandBuild.Execute();
            result.ExitCode.ShouldBe(0);
            result = MiniCoverRunner.ExecuteInstrumenter(toolPath,workdir);
            result.ExitCode.ShouldBe(0);
            result.StdErr.ShouldBeNullOrEmpty();

            var coverageJson = StringConverger.ApplyCleanup(File.ReadAllText(Path.Combine(toolPath, workdir, "coverage.json")), new Uri(sampleRootDirectory).LocalPath);
            ApprovaleHelper.VerifyJson(coverageJson);

            result = MiniCoverRunner.ExecuteReset(toolPath, workdir);
            result.ExitCode.ShouldBe(0);
            result.StdErr.ShouldBeNullOrEmpty();

            var testProjects = Directory.EnumerateFiles(Path.Combine(sampleRootDirectory, "test"), "*.csproj",
                SearchOption.AllDirectories);
            foreach (var testProject in testProjects.OrderBy(a => new FileInfo(a).Name))
            {
                var commandTest = Command.CreateDotNet("test", new[] {testProject, "--no-build"});
                commandTest.CaptureStdOut();
                commandTest.CaptureStdErr();
                result = commandTest.Execute();
                result.ExitCode.ShouldBe(0);
                result.StdErr.ShouldBeNullOrEmpty();
            }
            result = MiniCoverRunner.ExecuteUninstrument(toolPath, workdir);
            result.ExitCode.ShouldBe(0);
            result.StdErr.ShouldBeNullOrEmpty();

            var bytes = File.ReadAllBytes(Path.Combine(toolPath, workdir, "coverage-hits.txt"));
         
            ApprovaleHelper.VerifyByte(bytes);
        }
    }
}