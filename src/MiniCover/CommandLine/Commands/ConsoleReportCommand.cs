using System.Threading.Tasks;
using MiniCover.CommandLine.Options;
using MiniCover.Reports;

namespace MiniCover.CommandLine.Commands
{
    class ConsoleReportCommand : BaseCommand
    {
        private const string _name = "report";
        private const string _description = "Outputs coverage report";

        private readonly CoverageLoadedFileOption _coverageLoadedFileOption;
        private readonly ThresholdOption _thresholdOption;

        public ConsoleReportCommand(
            WorkingDirectoryOption workingDirectoryOption,
            CoverageLoadedFileOption coverageLoadedFileOption,
            ThresholdOption thresholdOption)
            : base(_name, _description)
        {
            _coverageLoadedFileOption = coverageLoadedFileOption;
            _thresholdOption = thresholdOption;

            Options = new IOption[]
            {
                workingDirectoryOption,
                coverageLoadedFileOption,
                thresholdOption
            };
        }

        protected override Task<int> Execute()
        {
            var consoleReport = new ConsoleReport();
            var result = consoleReport.Execute(_coverageLoadedFileOption.Result, _thresholdOption.Value);
            return Task.FromResult(result);
        }
    }
}