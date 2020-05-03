using System.Threading.Tasks;
using MiniCover.CommandLine.Options;
using MiniCover.Reports.Helpers;
using MiniCover.Reports.OpenCover;

namespace MiniCover.CommandLine.Commands
{
    class OpenCoverReportCommand : ICommand
    {
        private readonly WorkingDirectoryOption _workingDirectoryOption;
        private readonly CoverageLoadedFileOption _coverageLoadedFileOption;
        private readonly OpenCoverOutputOption _openCoverOutputOption;
        private readonly ThresholdOption _thresholdOption;

        public OpenCoverReportCommand(
            WorkingDirectoryOption workingDirectoryOption,
            CoverageLoadedFileOption coverageLoadedFileOption,
            OpenCoverOutputOption openCoverOutputOption,
            ThresholdOption thresholdOption)
        {
            _workingDirectoryOption = workingDirectoryOption;
            _coverageLoadedFileOption = coverageLoadedFileOption;
            _thresholdOption = thresholdOption;
            _openCoverOutputOption = openCoverOutputOption;
        }

        public string CommandName => "opencoverreport";
        public string CommandDescription => "Write an OpenCover-formatted XML report to file";
        public IOption[] Options => new IOption[]
        {
            _workingDirectoryOption,
            _coverageLoadedFileOption,
            _thresholdOption,
            _openCoverOutputOption
        };

        public Task<int> Execute()
        {
            OpenCoverReport.Execute(_coverageLoadedFileOption.Result, _openCoverOutputOption.FileInfo, _thresholdOption.Value);
            var summary = SummaryHelpers.CalculateSummary(_coverageLoadedFileOption.Result, _thresholdOption.Value);
            return Task.FromResult(summary.LinesCoveragePass ? 0 : 1);
        }
    }
}
