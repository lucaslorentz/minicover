using System.Threading.Tasks;
using MiniCover.CommandLine.Options;
using MiniCover.Reports.Clover;

namespace MiniCover.CommandLine.Commands
{
    public class CloverReportCommand : ICommand
    {
        private readonly IWorkingDirectoryOption _workingDirectoryOption;
        private readonly ICloverOutputOption _cloverOutputOption;
        private readonly ICoverageLoadedFileOption _coverageLoadedFileOption;
        private readonly ICloverReport _cloverReport;

        public CloverReportCommand(
            IWorkingDirectoryOption workingDirectoryOption,
            ICoverageLoadedFileOption coverageLoadedFileOption,
            ICloverOutputOption cloverOutputOption,
            ICloverReport cloverReport)
        {
            _workingDirectoryOption = workingDirectoryOption;
            _coverageLoadedFileOption = coverageLoadedFileOption;
            _cloverOutputOption = cloverOutputOption;
            _cloverReport = cloverReport;
        }

        public string CommandName => "cloverreport";
        public string CommandDescription => "Write an Clover-formatted XML report to file";

        public IOption[] Options => new IOption[]
        {
            _workingDirectoryOption,
            _coverageLoadedFileOption,
            _cloverOutputOption
        };

        public Task<int> Execute()
        {
            _cloverReport.Execute(_coverageLoadedFileOption.Result, _cloverOutputOption.FileInfo);
            return Task.FromResult(0);
        }
    }
}
