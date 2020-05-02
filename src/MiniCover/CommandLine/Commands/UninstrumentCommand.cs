using System.Threading.Tasks;
using MiniCover.CommandLine;
using MiniCover.CommandLine.Options;
using MiniCover.Instrumentation;

namespace MiniCover.Commands
{
    class UninstrumentCommand : BaseCommand
    {
        private const string _name = "uninstrument";
        private const string _description = "Uninstrument assemblies";

        private readonly CoverageLoadedFileOption _coverageLoadedFileOption;
        private readonly Uninstrumenter _uninstrumenter;

        public UninstrumentCommand(
            VerbosityOption verbosityOption,
            WorkingDirectoryOption workingDirectoryOption,
            CoverageLoadedFileOption coverageLoadedFileOption,
            Uninstrumenter uninstrumenter)
            : base(_name, _description)
        {
            _coverageLoadedFileOption = coverageLoadedFileOption;
            _uninstrumenter = uninstrumenter;
            Options = new IOption[]
            {
                verbosityOption,
                workingDirectoryOption,
                coverageLoadedFileOption
            };
        }

        protected override Task<int> Execute()
        {
            var result = _coverageLoadedFileOption.Result;
            _uninstrumenter.Execute(result);
            return Task.FromResult(0);
        }
    }
}
