using System.Threading.Tasks;
using MiniCover.CommandLine.Options;
using MiniCover.Reports.NCover;

namespace MiniCover.CommandLine.Commands
{
    public class NCoverReportCommand : ICommand
    {
        private readonly IWorkingDirectoryOption _workingDirectoryOption;
        private readonly ICoverageLoadedFileOption _coverageLoadedFileOption;
        private readonly INCoverOutputOption _nCoverOutputOption;
        private readonly INCoverReport _nCoverReport;

        public NCoverReportCommand(
            IWorkingDirectoryOption workingDirectoryOption,
            ICoverageLoadedFileOption coverageLoadedFileOption,
            INCoverOutputOption nCoverOutputOption,
            INCoverReport nCoverReport)
        {
            _workingDirectoryOption = workingDirectoryOption;
            _coverageLoadedFileOption = coverageLoadedFileOption;
            _nCoverOutputOption = nCoverOutputOption;
            _nCoverReport = nCoverReport;
        }

        public string CommandName => "xmlreport";
        public string CommandDescription => "Write an NCover-formatted XML report to file";

        public IOption[] Options => new IOption[]
        {
            _workingDirectoryOption,
            _coverageLoadedFileOption,
            _nCoverOutputOption
        };

        public Task<int> Execute()
        {
            _nCoverReport.Execute(_coverageLoadedFileOption.Result, _nCoverOutputOption.FileInfo);
            return Task.FromResult(0);
        }
    }
}
