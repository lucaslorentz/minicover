using System;
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

            var commands = serviceProvider.GetServices<BaseCommand>();
            foreach (var command in commands)
            {
                command.AddTo(commandLineApplication);
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

        private static IServiceProvider ConfigureServices(IOutput output)
        {
            var services = new ServiceCollection();

            services.AddSingleton<IOutput>(output);

            services.AddLogging(l => l
                .SetMinimumLevel(LogLevel.Trace)
                .AddProvider(new OutputLoggerProvider(output)));

            services.AddMemoryCache();

            services.AddTransient<BaseCommand, InstrumentCommand>();
            services.AddTransient<BaseCommand, UninstrumentCommand>();
            services.AddTransient<BaseCommand, ResetCommand>();
            services.AddTransient<BaseCommand, ConsoleReportCommand>();
            services.AddTransient<BaseCommand, HtmlReportCommand>();
            services.AddTransient<BaseCommand, NCoverReportCommand>();
            services.AddTransient<BaseCommand, OpenCoverReportCommand>();
            services.AddTransient<BaseCommand, CloverReportCommand>();
            services.AddTransient<BaseCommand, CoverallsReportCommand>();

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
            services.AddTransient<ThresholdOption>();
            services.AddTransient<CloverOutputOption>();
            services.AddTransient<HtmlOutputFolderOption>();
            services.AddTransient<NCoverOutputOption>();
            services.AddTransient<OpenCoverOutputOption>();
            services.AddTransient<VerbosityOption>();

            services.AddSingleton<Instrumenter>();
            services.AddSingleton<AssemblyInstrumenter>();
            services.AddSingleton<TypeInstrumenter>();
            services.AddSingleton<MethodInstrumenter>();

            services.AddSingleton<IFileReader, CachedFileReader>();

            return services.BuildServiceProvider();
        }
    }
}