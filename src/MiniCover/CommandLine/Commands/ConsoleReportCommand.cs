using System.Threading.Tasks;
using MiniCover.CommandLine.Options;
using MiniCover.Reports;

namespace MiniCover.CommandLine.Commands
{
    class ConsoleReportCommand : ICommand
    {
        private readonly WorkingDirectoryOption _workingDirectoryOption;
        private readonly CoverageLoadedFileOption _coverageLoadedFileOption;
        private readonly ThresholdOption _thresholdOption;

        public ConsoleReportCommand(
            WorkingDirectoryOption workingDirectoryOption,
            CoverageLoadedFileOption coverageLoadedFileOption,
            ThresholdOption thresholdOption)
        {
            _workingDirectoryOption = workingDirectoryOption;
            _coverageLoadedFileOption = coverageLoadedFileOption;
            _thresholdOption = thresholdOption;
        }

        public string CommandName => "report";
        public string CommandDescription => "Outputs coverage report";
        public IOption[] Options => new IOption[]
        {
            _workingDirectoryOption,
            _coverageLoadedFileOption,
            _thresholdOption
        };

        public Task<int> Execute()
        {
            var consoleReport = new ConsoleReport();
            var result = consoleReport.Execute(_coverageLoadedFileOption.Result, _thresholdOption.Value);
            return Task.FromResult(result);
        }
    }
}