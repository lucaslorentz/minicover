using System.Threading.Tasks;
using MiniCover.CommandLine.Options;
using MiniCover.Reports.Helpers;
using MiniCover.Reports.NCover;

namespace MiniCover.CommandLine.Commands
{
    class NCoverReportCommand : ICommand
    {
        private readonly WorkingDirectoryOption _workingDirectoryOption;
        private readonly CoverageLoadedFileOption _coverageLoadedFileOption;
        private readonly NCoverOutputOption _nCoverOutputOption;
        private readonly ThresholdOption _thresholdOption;

        public NCoverReportCommand(
            WorkingDirectoryOption workingDirectoryOption,
            CoverageLoadedFileOption coverageLoadedFileOption,
            NCoverOutputOption nCoverOutputOption,
            ThresholdOption thresholdOption)
        {
            _workingDirectoryOption = workingDirectoryOption;
            _coverageLoadedFileOption = coverageLoadedFileOption;
            _thresholdOption = thresholdOption;
            _nCoverOutputOption = nCoverOutputOption;
        }

        public string CommandName => "xmlreport";
        public string CommandDescription => "Write an NCover-formatted XML report to file";

        public IOption[] Options => new IOption[]
        {
            _workingDirectoryOption,
            _coverageLoadedFileOption,
            _thresholdOption,
            _nCoverOutputOption
        };

        public Task<int> Execute()
        {
            NCoverReport.Execute(_coverageLoadedFileOption.Result, _nCoverOutputOption.FileInfo, _thresholdOption.Value);
            var summary = SummaryHelpers.CalculateSummary(_coverageLoadedFileOption.Result, _thresholdOption.Value);
            return Task.FromResult(summary.LinesCoveragePass ? 0 : 1);
        }
    }
}
