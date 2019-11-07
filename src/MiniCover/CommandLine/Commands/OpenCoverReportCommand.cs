using System.Threading.Tasks;
using MiniCover.CommandLine.Options;
using MiniCover.Reports;
using MiniCover.Utils;

namespace MiniCover.CommandLine.Commands
{
    class OpenCoverReportCommand : BaseCommand
    {
        private const string _name = "opencoverreport";
        private const string _description = "Write an OpenCover-formatted XML report to file";

        private readonly CoverageLoadedFileOption _coverageLoadedFileOption;
        private readonly OpenCoverOutputOption _openCoverOutputOption;
        private readonly ThresholdOption _thresholdOption;

        public OpenCoverReportCommand(
            WorkingDirectoryOption workingDirectoryOption,
            CoverageLoadedFileOption coverageLoadedFileOption,
            OpenCoverOutputOption openCoverOutputOption,
            ThresholdOption thresholdOption)
        : base(_name, _description)
        {
            _coverageLoadedFileOption = coverageLoadedFileOption;
            _thresholdOption = thresholdOption;
            _openCoverOutputOption = openCoverOutputOption;

            Options = new IOption[]
            {
                workingDirectoryOption,
                _coverageLoadedFileOption,
                _thresholdOption,
                _openCoverOutputOption
            };
        }

        protected override Task<int> Execute()
        {
            OpenCoverReport.Execute(_coverageLoadedFileOption.Result, _openCoverOutputOption.Value, _thresholdOption.Value);
            var result = CalcUtils.IsHigherThanThreshold(_coverageLoadedFileOption.Result, _thresholdOption.Value);
            return Task.FromResult(result);
        }
    }
}
