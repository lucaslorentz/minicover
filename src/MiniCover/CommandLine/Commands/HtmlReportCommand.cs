using System.Threading.Tasks;
using MiniCover.CommandLine.Options;
using MiniCover.Reports.Html;

namespace MiniCover.CommandLine.Commands
{
    public class HtmlReportCommand : ICommand
    {
        private readonly IWorkingDirectoryOption _workingDirectoryOption;
        private readonly ICoverageLoadedFileOption _coverageLoadedFileOption;
        private readonly IHtmlOutputDirectoryOption _htmlOutputFolderOption;
        private readonly IThresholdOption _thresholdOption;
        private readonly IHtmlReport _htmlReport;

        public HtmlReportCommand(
            IWorkingDirectoryOption workingDirectoryOption,
            ICoverageLoadedFileOption coverageLoadedFileOption,
            IHtmlOutputDirectoryOption htmlOutputFolderOption,
            IThresholdOption thresholdOption,
            IHtmlReport htmlReport)
        {
            _workingDirectoryOption = workingDirectoryOption;
            _coverageLoadedFileOption = coverageLoadedFileOption;
            _thresholdOption = thresholdOption;
            _htmlReport = htmlReport;
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
            var result = _htmlReport.Execute(
                _coverageLoadedFileOption.Result,
                _htmlOutputFolderOption.DirectoryInfo,
                _thresholdOption.Value);

            return Task.FromResult(result);
        }
    }
}