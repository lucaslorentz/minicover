using System.Diagnostics;
using Microsoft.DotNet.Cli.Utils;
using Shouldly;

namespace MiniCover.ApprovalTests
{
    internal class MiniCoverRunner
    {
        public static CommandResult ExecuteInstrumenter(string toolsPath, string workdir)
        {
            return ExecuteMiniCover(toolsPath, "instrument",
                $"--workdir {workdir} --parentdir ../../ --assemblies **/bin/**/*.dll --sources **/*.cs");
        }

        public static CommandResult ExecuteReset(string toolsPath, string workdir)
        {
            return ExecuteMiniCover(toolsPath, "reset",
                $"--workdir {workdir}");
        }

        public static CommandResult ExecuteXmlReport(string toolsPath, string workdir)
        {
            return ExecuteMiniCover(toolsPath, "xmlreport",
                $"--workdir {workdir} --threshold 80");
        }

        public static CommandResult ExecuteUninstrument(string toolsPath, string workdir)
        {
            return ExecuteMiniCover(toolsPath, "uninstrument",
                $"--workdir {workdir}");
        }

        private static CommandResult ExecuteMiniCover(string minicoverDirectory, string command, string parameter)
        {
            StreamForwarder stdOut = new StreamForwarder();
            StreamForwarder stdError = new StreamForwarder();
            ProcessStartInfo processStartInfo = new ProcessStartInfo("dotnet",
                $"minicover {command} {parameter}")
            {
                WorkingDirectory = minicoverDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var process = new Process {StartInfo = processStartInfo};

            process.Start();
            var threadOut = stdOut.BeginRead(process.StandardOutput);
            var threadErr = stdError.BeginRead(process.StandardError);
            process.WaitForExit();
            threadOut.Wait();
            threadErr.Wait();
            process.HasExited.ShouldBe(true);
            return new CommandResult(processStartInfo, process.ExitCode, stdOut.CapturedOutput,
                stdError.CapturedOutput);
        }


        
    }
}