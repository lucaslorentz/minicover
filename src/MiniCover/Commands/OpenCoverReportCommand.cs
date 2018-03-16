using Microsoft.Extensions.CommandLineUtils;
using MiniCover.Commands.Options;
using MiniCover.Model;
using MiniCover.Reports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniCover.Commands
{
    internal class OpenCoverReportCommand : BaseCommandLineApplication
    {
        private readonly WorkingDirectoryOption _workingDirectoryOption = new WorkingDirectoryOption();
        private readonly IMiniCoverOption<InstrumentationResult> _coverageLoadedFileOption = new CoverageLoadedFileOption();
        private readonly ThresholdOption _thresholdOption = new ThresholdOption();
        private readonly OpenCoverOutputOption _openCoverOutputOption = new OpenCoverOutputOption();

        public OpenCoverReportCommand(CommandLineApplication parentCommandLineApplication)
            : base(parentCommandLineApplication)
        {
        }

        protected override string CommandDescription => "Write an OpenCover-formatted XML report to file";
        protected override string CommandName => "opencoverreport";
        protected override IEnumerable<IMiniCoverOption> MiniCoverOptions => new IMiniCoverOption[] { _workingDirectoryOption, _coverageLoadedFileOption, _thresholdOption, _openCoverOutputOption };

        protected override Task<int> Execution()
        {
            OpenCoverReport.Execute(_coverageLoadedFileOption.Value, _openCoverOutputOption.Value, _thresholdOption.Value);

            return Task.FromResult(0);
        }
    }
}