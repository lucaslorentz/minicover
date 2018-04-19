using MiniCover.Commands.Options;
using MiniCover.Model;
using MiniCover.Reports;
using System.Threading.Tasks;
using MiniCover.Utils;

namespace MiniCover.Commands.Reports
{
    internal class OpenCoverReportCommand : BaseCommand
    {
        private const string CommandDescription = "Write an OpenCover-formatted XML report to file";
        private const string CommandName = "opencoverreport";

        private static readonly IMiniCoverOption<InstrumentationResult> CoverageLoadedFileOption = new CoverageLoadedFileOption();
        private static readonly OpenCoverOutputOption OpenCoverOutputOption = new OpenCoverOutputOption();
        private static readonly ThresholdOption ThresholdOption = new ThresholdOption();
        private static readonly WorkingDirectoryOption WorkingDirectoryOption = new WorkingDirectoryOption();

        internal OpenCoverReportCommand()
        : base(CommandName, CommandDescription, WorkingDirectoryOption, CoverageLoadedFileOption, ThresholdOption, OpenCoverOutputOption)
        {
        }

        protected override Task<int> Execute()
        {
            OpenCoverReport.Execute(CoverageLoadedFileOption.GetValue(), OpenCoverOutputOption.GetValue(), ThresholdOption.GetValue());
            var result = CalcUtils.IsHigherThanThreshold(CoverageLoadedFileOption.GetValue(), ThresholdOption.GetValue());
            return Task.FromResult(result);
        }
    }
}
