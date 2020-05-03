using System.Threading.Tasks;
using MiniCover.CommandLine.Options;
using MiniCover.Reports.Clover;
using MiniCover.Reports.Helpers;

namespace MiniCover.CommandLine.Commands
{
    class CloverReportCommand : ICommand
    {
        private readonly WorkingDirectoryOption _workingDirectoryOption;
        private readonly CloverOutputOption _cloverOutputOption;
        private readonly CoverageLoadedFileOption _coverageLoadedFileOption;
        private readonly ThresholdOption _thresholdOption;

        public CloverReportCommand(
            WorkingDirectoryOption workingDirectoryOption,
            CloverOutputOption cloverOutputOption,
            CoverageLoadedFileOption coverageLoadedFileOption,
            ThresholdOption thresholdOption)
        {
            _workingDirectoryOption = workingDirectoryOption;
            _cloverOutputOption = cloverOutputOption;
            _coverageLoadedFileOption = coverageLoadedFileOption;
            _thresholdOption = thresholdOption;
        }

        public string CommandName => "cloverreport";
        public string CommandDescription => "Write an Clover-formatted XML report to file";

        public IOption[] Options => new IOption[]
        {
            _workingDirectoryOption,
            _coverageLoadedFileOption,
            _thresholdOption,
            _cloverOutputOption
        };

        public Task<int> Execute()
        {
            CloverReport.Execute(_coverageLoadedFileOption.Result, _cloverOutputOption.FileInfo, _thresholdOption.Value);
            var summary = SummaryHelpers.CalculateSummary(_coverageLoadedFileOption.Result, _thresholdOption.Value);
            return Task.FromResult(summary.LinesCoveragePass ? 0 : 1);
        }
    }
}
