using System.Threading.Tasks;
using MiniCover.CommandLine;
using MiniCover.CommandLine.Options;
using MiniCover.Instrumentation;

namespace MiniCover.Commands
{
    class UninstrumentCommand : ICommand
    {
        private readonly VerbosityOption _verbosityOption;
        private readonly WorkingDirectoryOption _workingDirectoryOption;
        private readonly CoverageLoadedFileOption _coverageLoadedFileOption;
        private readonly Uninstrumenter _uninstrumenter;

        public UninstrumentCommand(
            VerbosityOption verbosityOption,
            WorkingDirectoryOption workingDirectoryOption,
            CoverageLoadedFileOption coverageLoadedFileOption,
            Uninstrumenter uninstrumenter)
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
