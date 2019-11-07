using System.Threading.Tasks;
using MiniCover.CommandLine.Options;
using MiniCover.Reports;

namespace MiniCover.CommandLine.Commands
{
    class HtmlReportCommand : BaseCommand
    {
        private const string _name = "htmlreport";
        private const string _description = "Write html report to folder";

        private readonly CoverageLoadedFileOption _coverageLoadedFileOption;
        private readonly HtmlOutputFolderOption _htmlOutputFolderOption;
        private readonly ThresholdOption _thresholdOption;

        public HtmlReportCommand(
            WorkingDirectoryOption workingDirectoryOption,
            CoverageLoadedFileOption coverageLoadedFileOption,
            HtmlOutputFolderOption htmlOutputFolderOption,
            ThresholdOption thresholdOption)
            : base(_name, _description)
        {
            _coverageLoadedFileOption = coverageLoadedFileOption;
            _thresholdOption = thresholdOption;
            _htmlOutputFolderOption = htmlOutputFolderOption;

            Options = new IOption[]
            {
                workingDirectoryOption,
                coverageLoadedFileOption,
                thresholdOption,
                htmlOutputFolderOption
            };
        }

        protected override Task<int> Execute()
        {
            var consoleReport = new HtmlReport(_htmlOutputFolderOption.Value.FullName);
            var result = consoleReport.Execute(_coverageLoadedFileOption.Result, _thresholdOption.Value);
            return Task.FromResult(result);
        }
    }
}