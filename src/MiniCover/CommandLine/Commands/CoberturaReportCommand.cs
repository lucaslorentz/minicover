using System.Threading.Tasks;
using MiniCover.CommandLine.Options;
using MiniCover.Reports.Cobertura;

namespace MiniCover.CommandLine.Commands
{
    public class CoberturaReportCommand : ICommand
    {
        private readonly IWorkingDirectoryOption _workingDirectoryOption;
        private readonly ICoverageLoadedFileOption _coverageLoadedFileOption;
        private readonly ICoberturaOutputOption _coberturaOutputOption;
        private readonly ICoberturaReport _coberturaReport;

        public CoberturaReportCommand(
            IWorkingDirectoryOption workingDirectoryOption,
            ICoverageLoadedFileOption coverageLoadedFileOption,
            ICoberturaOutputOption coberturaOutputOption,
            ICoberturaReport coberturaReport)
        {
            _workingDirectoryOption = workingDirectoryOption;
            _coverageLoadedFileOption = coverageLoadedFileOption;
            _coberturaOutputOption = coberturaOutputOption;
            _coberturaReport = coberturaReport;
        }

        public string CommandName => "coberturareport";
        public string CommandDescription => "Write a Cobertura-formatted XML report to file";
        public IOption[] Options => new IOption[]
        {
            _workingDirectoryOption,
            _coverageLoadedFileOption,
            _coberturaOutputOption
        };

        public Task<int> Execute()
        {
            _coberturaReport.Execute(_coverageLoadedFileOption.Result, _coberturaOutputOption.FileInfo);
            return Task.FromResult(0);
        }
    }
}
