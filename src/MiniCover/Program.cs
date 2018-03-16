﻿using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using MiniCover.Commands;
using MiniCover.Instrumentation;
using MiniCover.Model;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace MiniCover
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var commandLineApplication = new CommandLineApplication();
            commandLineApplication.Name = "MiniCover";
            commandLineApplication.Description = "MiniCover - Code coverage for .NET Core via assembly instrumentation";

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

            commandLineApplication.Commands.Add(new UninstrumentCommand(commandLineApplication));

            commandLineApplication.Commands.Add(new ReportCommand(commandLineApplication));
            commandLineApplication.Commands.Add(new HtmlReportCommand(commandLineApplication));
            commandLineApplication.Commands.Add(new NCoverReportCommand(commandLineApplication));
            commandLineApplication.Commands.Add(new OpenCoverReportCommand(commandLineApplication));
            commandLineApplication.Commands.Add(new CloverReportCommand(commandLineApplication));

            commandLineApplication.Commands.Add(new ResetCommand(commandLineApplication));

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
    }
}