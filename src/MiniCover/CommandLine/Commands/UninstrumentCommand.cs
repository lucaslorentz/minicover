using System.Threading.Tasks;
using MiniCover.CommandLine;
using MiniCover.CommandLine.Options;
using MiniCover.Core.Instrumentation;

namespace MiniCover.Commands
{
    public class UninstrumentCommand : ICommand
    {
        private readonly IVerbosityOption _verbosityOption;
        private readonly IWorkingDirectoryOption _workingDirectoryOption;
        private readonly ICoverageLoadedFileOption _coverageLoadedFileOption;
        private readonly IUninstrumenter _uninstrumenter;

        public UninstrumentCommand(
            IVerbosityOption verbosityOption,
            IWorkingDirectoryOption workingDirectoryOption,
            ICoverageLoadedFileOption coverageLoadedFileOption,
            IUninstrumenter uninstrumenter)
        {
            _verbosityOption = verbosityOption;
            _workingDirectoryOption = workingDirectoryOption;
            _coverageLoadedFileOption = coverageLoadedFileOption;
            _uninstrumenter = uninstrumenter;
        }

        public string CommandName => "uninstrument";
        public string CommandDescription => "Uninstrument assemblies";
        public IOption[] Options => new IOption[]
        {
            _verbosityOption,
            _workingDirectoryOption,
            _coverageLoadedFileOption
        };

        public Task<int> Execute()
        {
            var result = _coverageLoadedFileOption.Result;
            _uninstrumenter.Execute(result);
            return Task.FromResult(0);
        }
    }
}
