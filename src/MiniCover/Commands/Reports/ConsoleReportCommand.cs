using MiniCover.Commands.Options;
using MiniCover.Model;
using MiniCover.Reports;
using System.Threading.Tasks;

namespace MiniCover.Commands.Reports
{
    internal class ConsoleReportCommand : BaseCommand
    {
        private const string CommandDescription = "Outputs coverage report";
        private const string CommandName = "report";

        private static readonly IMiniCoverOption<InstrumentationResult> CoverageLoadedFileOption = new CoverageLoadedFileOption();
        private static readonly ThresholdOption ThresholdOption = new ThresholdOption();
        private static readonly WorkingDirectoryOption WorkingDirectoryOption = new WorkingDirectoryOption();
        private static readonly VerbosityOption VerbosityOption = new VerbosityOption();

        internal ConsoleReportCommand()
            : base(CommandName, CommandDescription, WorkingDirectoryOption, CoverageLoadedFileOption, ThresholdOption, VerbosityOption)
        { }

        protected override Task<int> Execute()
        {
            var consoleReport = new ConsoleReport(VerbosityOption.GetValue());
            var result = consoleReport.Execute(CoverageLoadedFileOption.GetValue(), ThresholdOption.GetValue());

            return Task.FromResult(result);
        }
    }
}