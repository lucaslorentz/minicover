using System.Threading.Tasks;
using MiniCover.CommandLine.Options;
using MiniCover.Reports.OpenCover;

namespace MiniCover.CommandLine.Commands
{
    public class OpenCoverReportCommand : ICommand
    {
        private readonly IWorkingDirectoryOption _workingDirectoryOption;
        private readonly ICoverageLoadedFileOption _coverageLoadedFileOption;
        private readonly IOpenCoverOutputOption _openCoverOutputOption;
        private readonly IOpenCoverReport _openCoverReport;

        public OpenCoverReportCommand(
            IWorkingDirectoryOption workingDirectoryOption,
            ICoverageLoadedFileOption coverageLoadedFileOption,
            IOpenCoverOutputOption openCoverOutputOption,
            IOpenCoverReport openCoverReport)
        {
            _workingDirectoryOption = workingDirectoryOption;
            _coverageLoadedFileOption = coverageLoadedFileOption;
            _openCoverOutputOption = openCoverOutputOption;
            _openCoverReport = openCoverReport;
        }

        public string CommandName => "opencoverreport";
        public string CommandDescription => "Write an OpenCover-formatted XML report to file";
        public IOption[] Options => new IOption[]
        {
            _workingDirectoryOption,
            _coverageLoadedFileOption,
            _openCoverOutputOption
        };

        public Task<int> Execute()
        {
            _openCoverReport.Execute(_coverageLoadedFileOption.Result, _openCoverOutputOption.FileInfo);
            return Task.FromResult(0);
        }
    }
}
