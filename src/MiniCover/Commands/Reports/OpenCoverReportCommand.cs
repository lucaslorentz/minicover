using MiniCover.Commands.Options;
using MiniCover.Model;
using MiniCover.Reports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniCover.Commands.Reports
{
    internal class OpenCoverReportCommand : BaseCommand
    {
        private readonly IMiniCoverOption<InstrumentationResult> _coverageLoadedFileOption = new CoverageLoadedFileOption();
        private readonly OpenCoverOutputOption _openCoverOutputOption = new OpenCoverOutputOption();
        private readonly ThresholdOption _thresholdOption = new ThresholdOption();
        private readonly WorkingDirectoryOption _workingDirectoryOption = new WorkingDirectoryOption();
        protected override string CommandDescription => "Write an OpenCover-formatted XML report to file";
        protected override string CommandName => "opencoverreport";
        protected override IEnumerable<IMiniCoverOption> MiniCoverOptions => new IMiniCoverOption[] { _workingDirectoryOption, _coverageLoadedFileOption, _thresholdOption, _openCoverOutputOption };

        protected override Task<int> Execute()
        {
            OpenCoverReport.Execute(_coverageLoadedFileOption.GetValue(), _openCoverOutputOption.GetValue(), _thresholdOption.GetValue());

            return Task.FromResult(0);
        }
    }
}