using System.Threading.Tasks;
using MiniCover.CommandLine.Options;
using MiniCover.Reports;

namespace MiniCover.CommandLine.Commands
{
    public class ConsoleReportCommand : ICommand
    {
        private readonly IWorkingDirectoryOption _workingDirectoryOption;
        private readonly ICoverageLoadedFileOption _coverageLoadedFileOption;
        private readonly IThresholdOption _thresholdOption;
        private readonly INoFailOption _noFailOption;
        private readonly IConsoleReport _consoleReport;

        public ConsoleReportCommand(
            IWorkingDirectoryOption workingDirectoryOption,
            ICoverageLoadedFileOption coverageLoadedFileOption,
            IThresholdOption thresholdOption,
            INoFailOption noFailOption,
            IConsoleReport consoleReport)
        {
            _workingDirectoryOption = workingDirectoryOption;
            _coverageLoadedFileOption = coverageLoadedFileOption;
            _thresholdOption = thresholdOption;
            _noFailOption = noFailOption;
            _consoleReport = consoleReport;
        }

        public string CommandName => "report";
        public string CommandDescription => "Outputs coverage report";
        public IOption[] Options => new IOption[]
        {
            _workingDirectoryOption,
            _coverageLoadedFileOption,
            _thresholdOption,
            _noFailOption
        };

        public Task<int> Execute()
        {
            var result = _consoleReport.Execute(
                _coverageLoadedFileOption.Result,
                _thresholdOption.Value,
                _noFailOption.Value);

            return Task.FromResult(result);
        }
    }
}