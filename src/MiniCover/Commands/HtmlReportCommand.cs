using Microsoft.Extensions.CommandLineUtils;
using MiniCover.Commands.Options;
using MiniCover.Model;
using MiniCover.Reports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniCover.Commands
{
    internal class HtmlReportCommand : BaseCommandLineApplication
    {
        private readonly WorkingDirectoryOption _workingDirectoryOption = new WorkingDirectoryOption();
        private readonly IMiniCoverOption<InstrumentationResult> _coverageLoadedFileOption = new CoverageLoadedFileOption();
        private readonly ThresholdOption _thresholdOption = new ThresholdOption();
        private readonly HtmlOutputFolderOption _htmlOutputFolderOption = new HtmlOutputFolderOption();

        public HtmlReportCommand(CommandLineApplication parentCommandLineApplication)
            : base(parentCommandLineApplication)
        {
        }

        protected override string CommandDescription => "Write html report to folder";
        protected override string CommandName => "htmlreport";
        protected override IEnumerable<IMiniCoverOption> MiniCoverOptions => new IMiniCoverOption[] { _workingDirectoryOption, _coverageLoadedFileOption, _thresholdOption, _htmlOutputFolderOption };

        protected override Task<int> Execution()
        {
            var consoleReport = new HtmlReport(_htmlOutputFolderOption.Value.FullName);
            var result = consoleReport.Execute(_coverageLoadedFileOption.Value, _thresholdOption.Value);

            return Task.FromResult(result);
        }
    }
}