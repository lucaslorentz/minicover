using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MiniCover.CommandLine;
using MiniCover.CommandLine.Commands;
using MiniCover.CommandLine.Options;
using MiniCover.Commands;
using MiniCover.Exceptions;
using MiniCover.Infrastructure;
using MiniCover.Infrastructure.Console;
using MiniCover.Infrastructure.FileSystem;
using MiniCover.Instrumentation;
using MiniCover.Reports;
using MiniCover.Reports.Clover;
using MiniCover.Reports.Cobertura;
using MiniCover.Reports.Coveralls;
using MiniCover.Reports.Html;
using MiniCover.Reports.NCover;
using MiniCover.Reports.OpenCover;
using MiniCover.Utils;

namespace MiniCover
{
    class Program
    {
        private static int Main(string[] args)
        {
            var output = new ConsoleOutput();

            var serviceProvider = ConfigureServices(output);

            System.Console.OutputEncoding = Encoding.UTF8;

            var commandLineApplication = new CommandLineApplication();
            commandLineApplication.Name = "minicover";
            commandLineApplication.FullName = "MiniCover";
            commandLineApplication.Description = "MiniCover - Code coverage for .NET Core via assembly instrumentation";

            var commands = serviceProvider.GetServices<ICommand>();
            foreach (var command in commands)
            {
                commandLineApplication
                    .Command(command.CommandName, commandConfig =>
                    {
                        commandConfig.Description = command.CommandDescription;
                        commandConfig.HelpOption("-h | --help");

                        var prepareOptions = new List<Action>();

                        foreach (var option in command.Options)
                            prepareOptions.Add(AddOption(commandConfig, option));

                        commandConfig.OnExecute(() =>
                        {
                            foreach (var prepare in prepareOptions)
                                prepare();

                            return command.Execute();
                        });
                    });
            }

            commandLineApplication.HelpOption("-h | --help");

            commandLineApplication.VersionOption("--version", () =>
            {
                var assembly = Assembly.GetExecutingAssembly();
                var informationalVersionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                return informationalVersionAttribute.InformationalVersion;
            });

            commandLineApplication.OnExecute(() =>
            {
                commandLineApplication.ShowHelp();
                return 0;
            });

            try
            {
                return commandLineApplication.Execute(args);
            }
            catch (CommandParsingException ex)
            {
                output.WriteLine(ex.Message, LogLevel.Error);
                return 1;
            }
            catch (ValidationException ex)
            {
                output.WriteLine(ex.Message, LogLevel.Error);
                return 1;
            }
        }

        private static Action AddOption(CommandLineApplication command, IOption baseOption)
        {
            switch (baseOption)
            {
                case IMultiValueOption multiValueOption:
                    {
                        var option = command.Option(baseOption.Template, baseOption.Description, CommandOptionType.MultipleValue);
                        return () => multiValueOption.ReceiveValue(option.Values);
                    }
                case ISingleValueOption singleValueOption:
                    {
                        var option = command.Option(baseOption.Template, baseOption.Description, CommandOptionType.SingleValue);
                        return () => singleValueOption.ReceiveValue(option.Value());
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        private static IServiceProvider ConfigureServices(IOutput output)
        {
            var services = new ServiceCollection();

            services.AddSingleton<IOutput>(output);

            services.AddLogging(l => l
                .SetMinimumLevel(LogLevel.Trace)
                .AddProvider(new OutputLoggerProvider(output)));

            services.AddMemoryCache();

            services.AddTransient<ICommand, InstrumentCommand>();
            services.AddTransient<ICommand, UninstrumentCommand>();
            services.AddTransient<ICommand, ResetCommand>();
            services.AddTransient<ICommand, ConsoleReportCommand>();
            services.AddTransient<ICommand, HtmlReportCommand>();
            services.AddTransient<ICommand, NCoverReportCommand>();
            services.AddTransient<ICommand, OpenCoverReportCommand>();
            services.AddTransient<ICommand, CloverReportCommand>();
            services.AddTransient<ICommand, CoverallsReportCommand>();
            services.AddTransient<ICommand, CoberturaReportCommand>();

            services.AddTransient<IWorkingDirectoryOption, WorkingDirectoryOption>();
            services.AddTransient<ICoverageLoadedFileOption, CoverageLoadedFileOption>();
            services.AddTransient<ICoberturaOutputOption, CoberturaOutputOption>();
            services.AddTransient<ICloverOutputOption, CloverOutputOption>();
            services.AddTransient<INCoverOutputOption, NCoverOutputOption>();
            services.AddTransient<IOpenCoverOutputOption, OpenCoverOutputOption>();
            services.AddTransient<IHtmlOutputDirectoryOption, HtmlOutputDirectoryOption>();
            services.AddTransient<IThresholdOption, ThresholdOption>();
            services.AddTransient<IVerbosityOption, VerbosityOption>();

            services.AddTransient<WorkingDirectoryOption>();
            services.AddTransient<ParentDirectoryOption>();
            services.AddTransient<IncludeAssembliesPatternOption>();
            services.AddTransient<ExcludeAssembliesPatternOption>();
            services.AddTransient<IncludeSourcesPatternOption>();
            services.AddTransient<ExcludeSourcesPatternOption>();
            services.AddTransient<IncludeTestsPatternOption>();
            services.AddTransient<ExcludeTestsPatternOption>();
            services.AddTransient<HitsDirectoryOption>();
            services.AddTransient<CoverageFileOption>();
            services.AddTransient<CoverageLoadedFileOption>();
            services.AddTransient<HtmlOutputDirectoryOption>();
            services.AddTransient<VerbosityOption>();

            services.AddSingleton<Instrumenter>();
            services.AddSingleton<IUninstrumenter, Uninstrumenter>();
            services.AddSingleton<AssemblyInstrumenter>();
            services.AddSingleton<TypeInstrumenter>();
            services.AddSingleton<MethodInstrumenter>();

            services.AddSingleton<ICloverReport, CloverReport>();
            services.AddSingleton<ICoberturaReport, CoberturaReport>();
            services.AddSingleton<INCoverReport, NCoverReport>();
            services.AddSingleton<IOpenCoverReport, OpenCoverReport>();
            services.AddSingleton<IHtmlReport, HtmlReport>();
            services.AddSingleton<IConsoleReport, ConsoleReport>();
            services.AddSingleton<ICoverallsReport, CoverallsReport>();

            services.AddSingleton<DepsJsonUtils>();
            services.AddSingleton<IFileReader, CachedFileReader>();
            services.AddSingleton<IFileSystem, FileSystem>();

            return services.BuildServiceProvider();
        }
    }
}