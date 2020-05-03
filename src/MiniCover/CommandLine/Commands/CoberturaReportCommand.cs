using System.Threading.Tasks;
using MiniCover.CommandLine.Options;
using MiniCover.Reports.Cobertura;
using MiniCover.Reports.Helpers;

namespace MiniCover.CommandLine.Commands
{
    class CoberturaReportCommand : ICommand
    {
        private readonly WorkingDirectoryOption _workingDirectoryOption;
        private readonly CoverageLoadedFileOption _coverageLoadedFileOption;
        private readonly CoberturaOutputOption _coberturaOutputOption;
        private readonly ThresholdOption _thresholdOption;

        public CoberturaReportCommand(
            WorkingDirectoryOption workingDirectoryOption,
            CoverageLoadedFileOption coverageLoadedFileOption,
            CoberturaOutputOption coberturaOutputOption,
            ThresholdOption thresholdOption)
        {
            _workingDirectoryOption = workingDirectoryOption;
            _coverageLoadedFileOption = coverageLoadedFileOption;
            _thresholdOption = thresholdOption;
            _coberturaOutputOption = coberturaOutputOption;
        }

        public string CommandName => "coberturareport";
        public string CommandDescription => "Write a Cobertura-formatted XML report to file";
        public IOption[] Options => new IOption[]
        {
            _workingDirectoryOption,
            _coverageLoadedFileOption,
            _thresholdOption,
            _coberturaOutputOption
        };

        public Task<int> Execute()
        {
            new CoberturaReport().Execute(_coverageLoadedFileOption.Result, _coberturaOutputOption.FileInfo);
            var summary = SummaryHelpers.CalculateSummary(_coverageLoadedFileOption.Result, _thresholdOption.Value);
            return Task.FromResult(summary.LinesCoveragePass ? 0 : 1);
        }
    }
}
