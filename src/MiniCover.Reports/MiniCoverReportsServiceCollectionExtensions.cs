using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MiniCover.Reports.Clover;
using MiniCover.Reports.Cobertura;
using MiniCover.Reports.Console;
using MiniCover.Reports.Coveralls;
using MiniCover.Reports.Helpers;
using MiniCover.Reports.Html;
using MiniCover.Reports.NCover;
using MiniCover.Reports.OpenCover;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMiniCoverReports(this IServiceCollection services)
        {
            services.TryAddSingleton<IFileSystem, FileSystem>();

            services.TryAddSingleton<ISummaryFactory, SummaryFactory>();

            services.TryAddSingleton<ICloverReport, CloverReport>();
            services.TryAddSingleton<ICoberturaReport, CoberturaReport>();
            services.TryAddSingleton<INCoverReport, NCoverReport>();
            services.TryAddSingleton<IOpenCoverReport, OpenCoverReport>();
            services.TryAddSingleton<IHtmlReport, HtmlReport>();
            services.TryAddSingleton<IHtmlSourceFileReport, HtmlSourceFileReport>();
            services.TryAddSingleton<IConsoleReport, ConsoleReport>();
            services.TryAddSingleton<ICoverallsReport, CoverallsReport>();

            return services;
        }
    }
}
