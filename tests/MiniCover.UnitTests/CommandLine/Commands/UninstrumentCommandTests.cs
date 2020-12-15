using System.Threading.Tasks;
using FluentAssertions;
using MiniCover.CommandLine.Options;
using MiniCover.Commands;
using MiniCover.Core.Instrumentation;
using MiniCover.Core.Model;
using Moq;
using Xunit;

namespace MiniCover.UnitTests.CommandLine.Commands
{
    public class UninstrumentCommandTests : CommandTests<UninstrumentCommand>
    {
        private readonly Mock<IVerbosityOption> _verbosityOption;
        private readonly Mock<IWorkingDirectoryOption> _workingDirectoryOption;
        private readonly Mock<ICoverageLoadedFileOption> _coverageLoadedFileOption;
        private readonly Mock<IUninstrumenter> _uninstrumenter;

        public UninstrumentCommandTests()
        {
            _verbosityOption = MockFor<IVerbosityOption>();
            _workingDirectoryOption = MockFor<IWorkingDirectoryOption>();
            _coverageLoadedFileOption = MockFor<ICoverageLoadedFileOption>();
            _uninstrumenter = MockFor<IUninstrumenter>();

            Sut = new UninstrumentCommand(
                _verbosityOption.Object,
                _workingDirectoryOption.Object,
                _coverageLoadedFileOption.Object,
                _uninstrumenter.Object
            );
        }

        [Fact]
        public async Task Execute()
        {
            var result = new InstrumentationResult();

            _coverageLoadedFileOption.SetupGet(x => x.Result).Returns(result);
            _uninstrumenter.Setup(x => x.Execute(result));

            var exitCode = await Sut.Execute();
            exitCode.Should().Be(0);
        }
    }
}
