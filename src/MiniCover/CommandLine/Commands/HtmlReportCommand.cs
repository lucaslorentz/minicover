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
        private readonly INoFailOption _noFailOption;
        private readonly IHtmlReport _htmlReport;

        public HtmlReportCommand(
            IWorkingDirectoryOption workingDirectoryOption,
            ICoverageLoadedFileOption coverageLoadedFileOption,
            IHtmlOutputDirectoryOption htmlOutputFolderOption,
            IThresholdOption thresholdOption,
            INoFailOption noFailOption,
            IHtmlReport htmlReport)
        {
            _workingDirectoryOption = workingDirectoryOption;
            _coverageLoadedFileOption = coverageLoadedFileOption;
            _thresholdOption = thresholdOption;
            _noFailOption = noFailOption;
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
            _noFailOption,
            _htmlOutputFolderOption
        };

        public Task<int> Execute()
        {
            var result = _htmlReport.Execute(
                _coverageLoadedFileOption.Result,
                _htmlOutputFolderOption.DirectoryInfo,
                _thresholdOption.Value,
                _noFailOption.Value);

            return Task.FromResult(result);
        }
    }
}