using Microsoft.Extensions.CommandLineUtils;
using MiniCover.Commands.Options;
using MiniCover.Model;
using MiniCover.Reports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniCover.Commands
{
    internal class NCoverReportCommand : BaseCommandLineApplication
    {
        private readonly WorkingDirectoryOption _workingDirectoryOption = new WorkingDirectoryOption();
        private readonly IMiniCoverOption<InstrumentationResult> _coverageLoadedFileOption = new CoverageLoadedFileOption();
        private readonly ThresholdOption _thresholdOption = new ThresholdOption();
        private readonly NCoverOutputOption _nCoverOutputOption = new NCoverOutputOption();

        public NCoverReportCommand(CommandLineApplication parentCommandLineApplication)
            : base(parentCommandLineApplication)
        {
        }

        protected override string CommandDescription => "Write an NCover-formatted XML report to file";
        protected override string CommandName => "xmlreport";
        protected override IEnumerable<IMiniCoverOption> MiniCoverOptions => new IMiniCoverOption[] { _workingDirectoryOption, _coverageLoadedFileOption, _thresholdOption, _nCoverOutputOption };

        protected override Task<int> Execution()
        {
            XmlReport.Execute(_coverageLoadedFileOption.Value, _nCoverOutputOption.Value, _thresholdOption.Value);

            return Task.FromResult(0);
        }
    }
}