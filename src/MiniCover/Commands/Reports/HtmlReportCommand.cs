using MiniCover.Commands.Options;
using MiniCover.Model;
using MiniCover.Reports;
using System.Threading.Tasks;

namespace MiniCover.Commands.Reports
{
    internal class HtmlReportCommand : BaseCommand
    {
        private const string CommandDescription = "Write html report to folder";
        private const string CommandName = "htmlreport";

        private static readonly IMiniCoverOption<InstrumentationResult> CoverageLoadedFileOption = new CoverageLoadedFileOption();
        private static readonly HtmlOutputFolderOption HtmlOutputFolderOption = new HtmlOutputFolderOption();
        private static readonly ThresholdOption ThresholdOption = new ThresholdOption();
        private static readonly WorkingDirectoryOption WorkingDirectoryOption = new WorkingDirectoryOption();

        public HtmlReportCommand()
        : base(CommandName, CommandDescription, WorkingDirectoryOption, CoverageLoadedFileOption, ThresholdOption, HtmlOutputFolderOption)
        {
        }

        protected override Task<int> Execute()
        {
            var consoleReport = new HtmlReport(HtmlOutputFolderOption.GetValue().FullName);
            var result = consoleReport.Execute(CoverageLoadedFileOption.GetValue(), ThresholdOption.GetValue());

            return Task.FromResult(result);
        }
    }
}