using MiniCover.Commands.Options;
using MiniCover.Model;
using MiniCover.Reports.Clover;
using System.Threading.Tasks;

namespace MiniCover.Commands.Reports
{
    internal class CloverReportCommand : BaseCommand
    {
        private const string CommandDescription = "Write an Clover-formatted XML report to file";
        private const string CommandName = "cloverreport";

        private static readonly IMiniCoverOption<string> CloverOutputOption = new CloverOutputOption();
        private static readonly IMiniCoverOption<InstrumentationResult> CoverageLoadedFileOption = new CoverageLoadedFileOption();
        private static readonly ThresholdOption ThresholdOption = new ThresholdOption();
        private static readonly WorkingDirectoryOption WorkingDirectoryOption = new WorkingDirectoryOption();

        internal CloverReportCommand()
        : base(CommandName, CommandDescription, WorkingDirectoryOption, CoverageLoadedFileOption, ThresholdOption, CloverOutputOption)
        {
        }

        protected override Task<int> Execute()
        {
            CloverReport.Execute(CoverageLoadedFileOption.GetValue(), CloverOutputOption.GetValue(), ThresholdOption.GetValue());

            return Task.FromResult(0);
        }
    }
}