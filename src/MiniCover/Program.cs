using System;
using System.CommandLine;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MiniCover.CommandLine;
using MiniCover.CommandLine.Commands;
using MiniCover.CommandLine.Options;
using MiniCover.Commands;
using MiniCover.Exceptions;
using MiniCover.IO;

namespace MiniCover
{
    class Program
    {
        private static async Task<int> Main(string[] args)
        {
            var output = new ConsoleOutput();

            var serviceProvider = ConfigureServices(output);

            Console.OutputEncoding = Encoding.UTF8;

            var rootCommand = new RootCommand("MiniCover - Code coverage for .NET Core via assembly instrumentation");

            var commands = serviceProvider.GetServices<ICommand>();
            foreach (var command in commands)
            {
                var cmd = new Command(command.CommandName, command.CommandDescription);

                Action<ParseResult> prepareOptions = null;

                foreach (var option in command.Options)
                {
                    var (opt, prepare) = CreateOption(option);
                    prepareOptions += prepare;

                    cmd.Add(opt);
                }

                cmd.SetAction((ParseResult parseResult) =>
                {
                    prepareOptions?.Invoke(parseResult);

                    return command.Execute();
                });

                rootCommand.Add(cmd);
            }

            try
            {
                var parseResult = rootCommand.Parse(args);
                return await parseResult.InvokeAsync();
            }
            catch (ValidationException ex)
            {
                output.WriteLine(ex.Message, LogLevel.Error);
                return 1;
            }
        }

        private static (Option, Action<ParseResult>) CreateOption(IOption baseOption)
        {
            var aliases = baseOption.ShortName != null
                ? new[] { baseOption.ShortName }
                : Array.Empty<string>();

            switch (baseOption)
            {
                case IMultiValueOption multiValueOption:
                    {
                        var option = new Option<string[]>(baseOption.Name, aliases);
                        option.Description = baseOption.Description;
                        return (option, (parseResult) => multiValueOption.ReceiveValue(parseResult.GetValue(option)));
                    }
                case ISingleValueOption singleValueOption:
                    {
                        var option = new Option<string>(baseOption.Name, aliases);
                        option.Description = baseOption.Description;
                        return (option, (parseResult) => singleValueOption.ReceiveValue(parseResult.GetValue(option)));
                    }
                case INoValueOption noValueOptions:
                    {
                        var option = new Option<bool>(baseOption.Name, aliases);
                        option.Description = baseOption.Description;
                        return (option, (parseResult) => noValueOptions.ReceiveValue(parseResult.GetValue(option)));
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        private static IServiceProvider ConfigureServices(IOutput output)
        {
            var services = new ServiceCollection();

            services.AddSingleton(output);

            services.AddLogging(l => l
                .SetMinimumLevel(LogLevel.Trace)
                .AddProvider(new OutputLoggerProvider(output)));

            services.AddMiniCoverCore();
            services.AddMiniCoverReports();

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
            services.AddTransient<INoFailOption, NoFailOption>();

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

            return services.BuildServiceProvider();
        }
    }
}