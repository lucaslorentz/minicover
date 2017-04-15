using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using MiniCover.Instrumentation;
using MiniCover.Reports;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace MiniCover
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var commandLineApplication = new CommandLineApplication();
            commandLineApplication.Name = "MiniCover";
            commandLineApplication.Description = "MiniCover - Code coverate for .NET Core via assembly instrumentation";

            commandLineApplication.Command("instrument", command =>
            {
                command.Description = "Instrument assemblies";

                var workDirOption = CreateWorkdirOption(command);
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

                    var assemblies = GetFiles(includeAssembliesOption, excludeAssembliesOption, "**/*.dll");
                    if (assemblies.Length == 0)
                        throw new Exception("No assemblies found");

                    var sourceFiles = GetFiles(includeSourceOption, excludeSourceOption, "**/*.cs");
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

            commandLineApplication.Command("report", command =>
            {
                command.Description = "Outputs coverage report";

                var workDirOption = CreateWorkdirOption(command);
                var coverageFileOption = CreateCoverageFileOption(command);
                var thresholdOption = CreateThresholdOption(command);
                command.HelpOption("-h | --help");

                command.OnExecute(() =>
                {
                    UpdateWorkingDirectory(workDirOption);

                    var coverageFile = GetCoverageFile(coverageFileOption);
                    var threshold = GetThreshold(thresholdOption);
                    var result = LoadCoverageFile(coverageFile);
                    return ConsoleReport.Execute(result, threshold);
                });
            });

            commandLineApplication.Command("htmlreport", command =>
            {
                command.Description = "Write html report to folder";

                var workDirOption = CreateWorkdirOption(command);
                var coverageFileOption = CreateCoverageFileOption(command);
                var thresholdOption = CreateThresholdOption(command);
                var outputOption = command.Option("--output", "Output folder for html report [default: coverage-html]", CommandOptionType.SingleValue);
                command.HelpOption("-h | --help");

                command.OnExecute(() =>
                {
                    UpdateWorkingDirectory(workDirOption);

                    var coverageFile = GetCoverageFile(coverageFileOption);
                    var threshold = GetThreshold(thresholdOption);
                    var result = LoadCoverageFile(coverageFile);
                    var output = GetHtmlReportOutput(outputOption);
                    HtmlReport.Execute(result, threshold);
                    return 0;
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
            if (workDirOption.Value() != null)
            {
                var fullWorkDir = Path.GetFullPath(workDirOption.Value());
                Console.WriteLine($"Working directory: {fullWorkDir}");
                Directory.SetCurrentDirectory(fullWorkDir);
            }

            return Directory.GetCurrentDirectory();
        }

        private static CommandOption CreateThresholdOption(CommandLineApplication command)
        {
            return command.Option("--threshold", "Coverage percentage threshold (default: 90)", CommandOptionType.SingleValue);
        }

        private static CommandOption CreateCoverageFileOption(CommandLineApplication command)
        {
            return command.Option("--coverage-file", "Coverage file name [default: coverage.json]", CommandOptionType.SingleValue);
        }

        private static object GetHtmlReportOutput(CommandOption outputOption)
        {
            return outputOption.Value() ?? "coverage-html";
        }

        private static string GetCoverageFile(CommandOption coverageFileOption)
        {
            return coverageFileOption.Value() ?? "coverage.json";
        }

        private static string GetHitsFile(CommandOption hitsFileOption)
        {
            return Path.GetFullPath(hitsFileOption.Value() ?? "coverage-hits.txt");
        }

        private static string GetSourceDirectory(CommandOption sourceOption)
        {
            return Path.GetFullPath(sourceOption.Value() ?? Directory.GetCurrentDirectory());
        }

        private static string[] GetFiles(CommandOption includeOption, CommandOption excludeOption, string defaultInclude)
        {
            var matcher = new Microsoft.Extensions.FileSystemGlobbing.Matcher();

            if (includeOption.HasValue())
            {
                foreach (var include in includeOption.Values)
                {
                    matcher.AddInclude(include);
                }
            }
            else if (!string.IsNullOrEmpty(defaultInclude))
            {
                matcher.AddInclude(defaultInclude);
            }

            foreach (var exclude in excludeOption.Values)
            {
                matcher.AddExclude(exclude);
            }

            var currentDirectoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            var directoryInfoWrapper = new DirectoryInfoWrapper(currentDirectoryInfo);

            var fileMatchResult = matcher.Execute(directoryInfoWrapper);
            return fileMatchResult.Files.Select(f => Path.GetFullPath(f.Path)).ToArray();
        }

        private static void SaveCoverageFile(string coverageFile, InstrumentationResult result)
        {
            File.WriteAllText(coverageFile, JsonConvert.SerializeObject(result, Formatting.Indented));
        }

        private static InstrumentationResult LoadCoverageFile(string coverageFile)
        {
            if (!File.Exists(coverageFile))
                throw new Exception($"Coverage file {coverageFile} doesn't exist");

            return JsonConvert.DeserializeObject<InstrumentationResult>(File.ReadAllText(coverageFile));
        }

        private static float GetThreshold(CommandOption thresholdOption)
        {
            return float.Parse(thresholdOption.Value() ?? "90", CultureInfo.InvariantCulture) / 100;
        }
    }
}