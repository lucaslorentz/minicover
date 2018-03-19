using MiniCover.Commands.Options;
using MiniCover.Model;
using MiniCover.Reports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniCover.Commands.Reports
{
    internal class ConsoleReportCommand : BaseCommand
    {
        private readonly WorkingDirectoryOption _workingDirectoryOption = new WorkingDirectoryOption();
        private readonly IMiniCoverOption<InstrumentationResult> _coverageLoadedFileOption = new CoverageLoadedFileOption();
        private readonly ThresholdOption _thresholdOption = new ThresholdOption();

        protected override string CommandDescription => "Outputs coverage report";
        protected override string CommandName => "report";
        protected override IEnumerable<IMiniCoverOption> MiniCoverOptions => new IMiniCoverOption[] { _workingDirectoryOption, _coverageLoadedFileOption, _thresholdOption };

        protected override Task<int> Execute()
        {
            var consoleReport = new ConsoleReport();
            var result = consoleReport.Execute(_coverageLoadedFileOption.Value, _thresholdOption.Value);

            return Task.FromResult(result);
        }
    }
}