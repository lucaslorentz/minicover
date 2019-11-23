using System.Threading.Tasks;
using MiniCover.CommandLine.Options;
using MiniCover.Reports;
using MiniCover.Utils;

namespace MiniCover.CommandLine.Commands
{
    class CoberturaReportCommand : BaseCommand
    {
        private const string _name = "coberturareport";
        private const string _description = "Write a Cobertura-formatted XML report to file";

        private readonly CoverageLoadedFileOption _coverageLoadedFileOption;
        private readonly CoberturaOutputOption _coberturaOutputOption;
        private readonly ThresholdOption _thresholdOption;

        public CoberturaReportCommand(
            WorkingDirectoryOption workingDirectoryOption,
            CoverageLoadedFileOption coverageLoadedFileOption,
            CoberturaOutputOption coberturaOutputOption,
            ThresholdOption thresholdOption)
        : base(_name, _description)
        {
            _coverageLoadedFileOption = coverageLoadedFileOption;
            _thresholdOption = thresholdOption;
            _coberturaOutputOption = coberturaOutputOption;

            Options = new IOption[]
            {
                workingDirectoryOption,
                _coverageLoadedFileOption,
                _thresholdOption,
                _coberturaOutputOption
            };
        }

        protected override Task<int> Execute()
        {
            new CoberturaReport().Execute(_coverageLoadedFileOption.Result, _coberturaOutputOption.Value);
            var result = CalcUtils.IsHigherThanThreshold(_coverageLoadedFileOption.Result, _thresholdOption.Value);
            return Task.FromResult(result);
        }
    }
}
