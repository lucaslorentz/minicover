using MiniCover.Commands.Options;
using MiniCover.Model;
using MiniCover.Reports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniCover.Commands.Reports
{
    internal class HtmlReportCommand : BaseCommand
    {
        private readonly IMiniCoverOption<InstrumentationResult> _coverageLoadedFileOption =
            new CoverageLoadedFileOption();

        private readonly HtmlOutputFolderOption _htmlOutputFolderOption = new HtmlOutputFolderOption();
        private readonly ThresholdOption _thresholdOption = new ThresholdOption();
        private readonly WorkingDirectoryOption _workingDirectoryOption = new WorkingDirectoryOption();
        protected override string CommandDescription => "Write html report to folder";
        protected override string CommandName => "htmlreport";

        protected override IEnumerable<IMiniCoverOption> MiniCoverOptions => new IMiniCoverOption[]
            {_workingDirectoryOption, _coverageLoadedFileOption, _thresholdOption, _htmlOutputFolderOption};

        protected override Task<int> Execute()
        {
            var consoleReport = new HtmlReport(_htmlOutputFolderOption.GetValue().FullName);
            var result = consoleReport.Execute(_coverageLoadedFileOption.GetValue(), _thresholdOption.GetValue());

            return Task.FromResult(result);
        }
    }
}