using MiniCover.Commands.Options;
using MiniCover.Model;
using MiniCover.Reports;
using System.Threading.Tasks;

namespace MiniCover.Commands.Reports
{
    internal class NCoverReportCommand : BaseCommand
    {
        private const string CommandDescription = "Write an NCover-formatted XML report to file";
        private const string CommandName = "xmlreport";

        private static readonly IMiniCoverOption<InstrumentationResult> CoverageLoadedFileOption = new CoverageLoadedFileOption();
        private static readonly NCoverOutputOption NCoverOutputOption = new NCoverOutputOption();
        private static readonly ThresholdOption ThresholdOption = new ThresholdOption();
        private static readonly WorkingDirectoryOption WorkingDirectoryOption = new WorkingDirectoryOption();

        internal NCoverReportCommand()
        : base(CommandName, CommandDescription, WorkingDirectoryOption, CoverageLoadedFileOption, ThresholdOption, NCoverOutputOption)
        {
        }

        protected override Task<int> Execute()
        {
            var result = XmlReport.Execute(CoverageLoadedFileOption.GetValue(), NCoverOutputOption.GetValue(), ThresholdOption.GetValue());

            return Task.FromResult(result);
        }
    }
}
