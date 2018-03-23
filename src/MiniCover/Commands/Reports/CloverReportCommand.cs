using MiniCover.Commands.Options;
using MiniCover.Model;
using MiniCover.Reports.Clover;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniCover.Commands.Reports
{
    internal class CloverReportCommand : BaseCommand
    {
        private readonly WorkingDirectoryOption _workingDirectoryOption = new WorkingDirectoryOption();
        private readonly IMiniCoverOption<InstrumentationResult> _coverageLoadedFileOption = new CoverageLoadedFileOption();
        private readonly ThresholdOption _thresholdOption = new ThresholdOption();
        private readonly IMiniCoverOption<string> _cloverOutputOption = new CloverOutputOption();

        protected override string CommandDescription => "Write an Clover-formatted XML report to file";
        protected override string CommandName => "cloverreport";
        protected override IEnumerable<IMiniCoverOption> MiniCoverOptions => new IMiniCoverOption[] { _workingDirectoryOption, _coverageLoadedFileOption, _thresholdOption, _cloverOutputOption };

        protected override Task<int> Execute()
        {
            CloverReport.Execute(_coverageLoadedFileOption.GetValue(), _cloverOutputOption.GetValue(), _thresholdOption.GetValue());

            return Task.FromResult(0);
        }
    }
}