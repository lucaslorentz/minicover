using System.Threading.Tasks;
using MiniCover.CommandLine.Options;
using MiniCover.Reports.Clover;
using MiniCover.Utils;

namespace MiniCover.CommandLine.Commands
{
    class CloverReportCommand : BaseCommand
    {
        private const string _name = "cloverreport";
        private const string _description = "Write an Clover-formatted XML report to file";

        private readonly CloverOutputOption _cloverOutputOption;
        private readonly CoverageLoadedFileOption _coverageLoadedFileOption;
        private readonly ThresholdOption _thresholdOption;

        public CloverReportCommand(
            WorkingDirectoryOption workingDirectoryOption,
            CloverOutputOption cloverOutputOption,
            CoverageLoadedFileOption coverageLoadedFileOption,
            ThresholdOption thresholdOption)
            : base(_name, _description)
        {
            _cloverOutputOption = cloverOutputOption;
            _coverageLoadedFileOption = coverageLoadedFileOption;
            _thresholdOption = thresholdOption;

            Options = new IOption[]
            {
                workingDirectoryOption,
                coverageLoadedFileOption,
                thresholdOption,
                cloverOutputOption
            };
        }

        protected override Task<int> Execute()
        {
            CloverReport.Execute(_coverageLoadedFileOption.Result, _cloverOutputOption.Value, _thresholdOption.Value);
            var result = CalcUtils.IsHigherThanThreshold(_coverageLoadedFileOption.Result, _thresholdOption.Value);
            return Task.FromResult(result);
        }
    }
}
