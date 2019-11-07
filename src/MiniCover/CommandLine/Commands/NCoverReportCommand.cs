using System.Threading.Tasks;
using MiniCover.CommandLine.Options;
using MiniCover.Reports;
using MiniCover.Utils;

namespace MiniCover.CommandLine.Commands
{
    class NCoverReportCommand : BaseCommand
    {
        private const string _name = "xmlreport";
        private const string _description = "Write an NCover-formatted XML report to file";

        private readonly CoverageLoadedFileOption _coverageLoadedFileOption;
        private readonly NCoverOutputOption _nCoverOutputOption;
        private readonly ThresholdOption _thresholdOption;

        public NCoverReportCommand(
            WorkingDirectoryOption workingDirectoryOption,
            CoverageLoadedFileOption coverageLoadedFileOption,
            NCoverOutputOption nCoverOutputOption,
            ThresholdOption thresholdOption)
        : base(_name, _description)
        {
            _coverageLoadedFileOption = coverageLoadedFileOption;
            _thresholdOption = thresholdOption;
            _nCoverOutputOption = nCoverOutputOption;

            Options = new IOption[]
            {
                workingDirectoryOption,
                _coverageLoadedFileOption,
                _thresholdOption,
                _nCoverOutputOption
            };
        }

        protected override Task<int> Execute()
        {
            XmlReport.Execute(_coverageLoadedFileOption.Result, _nCoverOutputOption.Value, _thresholdOption.Value);
            var result = CalcUtils.IsHigherThanThreshold(_coverageLoadedFileOption.Result, _thresholdOption.Value);
            return Task.FromResult(result);
        }
    }
}
