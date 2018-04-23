using Microsoft.Extensions.CommandLineUtils;
using MiniCover.Commands;
using MiniCover.Commands.Reports;
using MiniCover.Instrumentation;
using MiniCover.Model;
using MiniCover.Reports.Coveralls;
using MiniCover.Utils;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace MiniCover
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var commandLineApplication = new CommandLineApplication
            {
                Name = "MiniCover",
                Description = "MiniCover - Code coverage for .NET Core via assembly instrumentation"
            };

            commandLineApplication.Command("instrument", command =>
            {
                command.Description = "Instrument assemblies";

                var workDirOption = CreateWorkdirOption(command);
                var parentDirOption = command.Option("--parentdir", "Set parent directory for assemblies and source directories (if not used, falls back to --workdir)", CommandOptionType.SingleValue);
                var includeAssembliesOption = command.Option("--assemblies", "Pattern to include assemblies [default: **/*.dll]", CommandOptionType.MultipleValue);
                var excludeAssembliesOption = command.Option("--exclude-assemblies", "Pattern to exclude assemblies", CommandOptionType.MultipleValue);
                var includeSourceOption = command.Option("--sources", "Pattern to include source files [default: **/*]", CommandOptionType.MultipleValue);
                var excludeSourceOption = command.Option("--exclude-sources", "Pattern to exclude source files", CommandOptionType.MultipleValue);
                var hitsFileOption = command.Option("--hits-file", "Hits file name [default: coverage-hits.txt]", CommandOptionType.SingleValue);

                var coverageFileOption = CreateCoverageFileOption(command);

                command.HelpOption("-h | --help");

                command.OnExecute(() =>
                {
                    var workdir = UpdateWorkingDirectory(workDirOption);

                    var assemblies = FileUtils.GetFiles(
                        includeAssembliesOption.HasValue() ? includeAssembliesOption.Values : null,
                        excludeAssembliesOption.HasValue() ? includeAssembliesOption.Values : null,
                        "**/*.dll",
                        parentDirOption.Value());
                    if (assemblies.Length == 0)
                        throw new Exception("No assemblies found");

                    var sourceFiles = FileUtils.GetFiles(
                        includeSourceOption.HasValue() ? includeSourceOption.Values : null,
                        excludeSourceOption.HasValue() ? excludeSourceOption.Values : null,
                        "**/*.cs",
                        parentDirOption.Value());
                    if (sourceFiles.Length == 0)
                        throw new Exception("No source files found");

                    var hitsFile = GetHitsFile(hitsFileOption);
                    var coverageFile = GetCoverageFile(coverageFileOption);
                    var instrumenter = new Instrumenter(assemblies, hitsFile, sourceFiles, workdir);
                    var result = instrumenter.Execute();
                    SaveCoverageFile(coverageFile, result);
                    return 0;
                });
            });

            commandLineApplication.Command("uninstrument", command =>
            {
                command.Description = "Uninstrument assemblies";

                var workDirOption = CreateWorkdirOption(command);
                var coverageFileOption = CreateCoverageFileOption(command);
                command.HelpOption("-h | --help");

                command.OnExecute(() =>
                {
                    UpdateWorkingDirectory(workDirOption);

                    var coverageFile = GetCoverageFile(coverageFileOption);
                    var result = LoadCoverageFile(coverageFile);
                    Uninstrumenter.Execute(result);
                    return 0;
                });
            });

            new ResetCommand().AddTo(commandLineApplication);

            new ConsoleReportCommand().AddTo(commandLineApplication);
            new HtmlReportCommand().AddTo(commandLineApplication);
            new NCoverReportCommand().AddTo(commandLineApplication);
            new OpenCoverReportCommand().AddTo(commandLineApplication);
            new CloverReportCommand().AddTo(commandLineApplication);

            commandLineApplication.Command("coverallsreport", command =>
            {
                command.Description = "Write a coveralls-formatted JSON report to folder";
                var rootPathOption = command.Option("--root-path", "Set the git root path", CommandOptionType.SingleValue);
                var outputOption = command.Option("--output", "Output file for coveralls report", CommandOptionType.SingleValue);
                var serviceJobIdOption = command.Option("--service-job-id", "Define service_job_id in coveralls json", CommandOptionType.SingleValue);
                var serviceNameOption = command.Option("--service-name", "Define service_name in coveralls json", CommandOptionType.SingleValue);
                var repoTokenOption = command.Option("--repo-token", "set the repo token", CommandOptionType.SingleValue);
                var commitOption = command.Option("--commit", "set the git commit id", CommandOptionType.SingleValue);
                var commitMessageOption = command.Option("--commit-message", "set the commit message", CommandOptionType.SingleValue);
                var commitAuthorNameOption = command.Option("--commit-author-name", "set the commit author name", CommandOptionType.SingleValue);
                var commitAuthorEmailOption = command.Option("--commit-author-email", "set the commit author email", CommandOptionType.SingleValue);
                var commitCommitterNameOption = command.Option("--commit-committer-name", "set the commit committer name", CommandOptionType.SingleValue);
                var commitCommitterEmailOption = command.Option("--commit-committer-email", "set the commit committer email", CommandOptionType.SingleValue);
                var branchOption = command.Option("--branch", "set the git branch", CommandOptionType.SingleValue);
                var remoteOption = command.Option("--remote", "set the git remote name", CommandOptionType.SingleValue);
                var remoteUrlOption = command.Option("--remote-url", "set the git remote url", CommandOptionType.SingleValue);

                var workDirOption = CreateWorkdirOption(command);
                var coverageFileOption = CreateCoverageFileOption(command);

                command.HelpOption("-h | --help");

                command.OnExecute(() =>
                {
                    UpdateWorkingDirectory(workDirOption);

                    var coverageFile = GetCoverageFile(coverageFileOption);
                    var result = LoadCoverageFile(coverageFile);
                    var output = outputOption.Value();

                    var rootPath = rootPathOption.HasValue()
                        ? Path.GetFullPath(rootPathOption.Value())
                        : Directory.GetCurrentDirectory();

                    var report = new CoverallsReport(
                        output,
                        repoTokenOption.Value(),
                        serviceJobIdOption.Value(),
                        serviceNameOption.Value(),
                        commitMessageOption.Value(),
                        rootPath,
                        commitOption.Value(),
                        commitAuthorNameOption.Value(),
                        commitAuthorEmailOption.Value(),
                        commitCommitterNameOption.Value(),
                        commitCommitterEmailOption.Value(),
                        branchOption.Value(),
                        remoteOption.Value(),
                        remoteUrlOption.Value()
                    );

                    return report.Execute(result).Result;
                });

            });

            commandLineApplication.HelpOption("-h | --help");
            commandLineApplication.OnExecute(() =>
            {
                commandLineApplication.ShowHelp();
                return 0;
            });

            return commandLineApplication.Execute(args);
        }

        private static CommandOption CreateWorkdirOption(CommandLineApplication command)
        {
            return command.Option("--workdir", "Change working directory", CommandOptionType.SingleValue);
        }

        private static string UpdateWorkingDirectory(CommandOption workDirOption)
        {
            if (workDirOption.HasValue())
            {
                var fullWorkDir = Path.GetFullPath(workDirOption.Value());
                if (!File.Exists(fullWorkDir))
                {
                    Directory.CreateDirectory(fullWorkDir);
                }
                Directory.SetCurrentDirectory(fullWorkDir);
            }
            return Directory.GetCurrentDirectory();
        }

        private static CommandOption CreateCoverageFileOption(CommandLineApplication command)
        {
            return command.Option("--coverage-file", "Coverage file name [default: coverage.json]", CommandOptionType.SingleValue);
        }

        private static string GetCoverageFile(CommandOption coverageFileOption)
        {
            return coverageFileOption.Value() ?? "coverage.json";
        }

        private static string GetHitsFile(CommandOption hitsFileOption)
        {
            return Path.GetFullPath(hitsFileOption.Value() ?? "coverage-hits.txt");
        }
        
        private static void SaveCoverageFile(string coverageFile, InstrumentationResult result)
        {
            File.WriteAllText(coverageFile, JsonConvert.SerializeObject(result, Formatting.Indented));
        }

        private static InstrumentationResult LoadCoverageFile(string coverageFile)
        {
            if (!File.Exists(coverageFile))
                throw new FileNotFoundException($"Coverage file {coverageFile} doesn't exist");

            return JsonConvert.DeserializeObject<InstrumentationResult>(File.ReadAllText(coverageFile));
        }
    }
}