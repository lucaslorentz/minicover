using System.Threading.Tasks;
using MiniCover.CommandLine.Options;
using MiniCover.Reports.Html;

namespace MiniCover.CommandLine.Commands
{
    class HtmlReportCommand : ICommand
    {
        private readonly WorkingDirectoryOption _workingDirectoryOption;
        private readonly CoverageLoadedFileOption _coverageLoadedFileOption;
        private readonly HtmlOutputDirectoryOption _htmlOutputFolderOption;
        private readonly ThresholdOption _thresholdOption;

        public HtmlReportCommand(
            WorkingDirectoryOption workingDirectoryOption,
            CoverageLoadedFileOption coverageLoadedFileOption,
            HtmlOutputDirectoryOption htmlOutputFolderOption,
            ThresholdOption thresholdOption)
        {
            _workingDirectoryOption = workingDirectoryOption;
            _coverageLoadedFileOption = coverageLoadedFileOption;
            _thresholdOption = thresholdOption;
            _htmlOutputFolderOption = htmlOutputFolderOption;
        }

        public string CommandName => "htmlreport";
        public string CommandDescription => "Write html report to folder";

        public IOption[] Options => new IOption[]
        {
            _workingDirectoryOption,
            _coverageLoadedFileOption,
            _thresholdOption,
            _htmlOutputFolderOption
        };

        public Task<int> Execute()
        {
            var consoleReport = new HtmlReport(_htmlOutputFolderOption.DirectoryInfo.FullName);
            var result = consoleReport.Execute(_coverageLoadedFileOption.Result, _thresholdOption.Value);
            return Task.FromResult(result);
        }
    }
}