using System.IO.Abstractions;
using System.Threading.Tasks;
using FluentAssertions;
using MiniCover.CommandLine.Commands;
using MiniCover.CommandLine.Options;
using MiniCover.Core.Model;
using MiniCover.Reports.Console;
using Moq;
using Xunit;

namespace MiniCover.UnitTests.CommandLine.Commands
{
    public class ConsoleReportCommandTests : CommandTests<ConsoleReportCommand>
    {
        private readonly Mock<IWorkingDirectoryOption> _workingDirectoryOption;
        private readonly Mock<ICoverageLoadedFileOption> _coverageLoadedFileOption;
        private readonly Mock<IThresholdOption> _thresholdOption;
        private readonly Mock<INoFailOption> _noFailOption;
        private readonly Mock<IConsoleReport> _consoleReport;

        public ConsoleReportCommandTests()
        {
            _workingDirectoryOption = MockFor<IWorkingDirectoryOption>();
            _coverageLoadedFileOption = MockFor<ICoverageLoadedFileOption>();
            _thresholdOption = MockFor<IThresholdOption>();
            _noFailOption = MockFor<INoFailOption>();
            _consoleReport = MockFor<IConsoleReport>();

            Sut = new ConsoleReportCommand(
                _workingDirectoryOption.Object,
                _coverageLoadedFileOption.Object,
                _thresholdOption.Object,
                _noFailOption.Object,
                _consoleReport.Object
            );
        }

        [InlineData(50f, false)]
        [InlineData(75f, true)]
        [Theory]
        public async Task Execute(float threshold, bool noFail)
        {
            var result = new InstrumentationResult();
            var output = MockFor<IDirectoryInfo>();

            _coverageLoadedFileOption.SetupGet(x => x.Result).Returns(result);
            _thresholdOption.SetupGet(x => x.Value).Returns(threshold);
            _noFailOption.SetupGet(x => x.Value).Returns(noFail);
            _consoleReport.Setup(x => x.Execute(result, threshold, noFail))
                .Returns(0);

            var exitCode = await Sut.Execute();
            exitCode.Should().Be(0);
        }
    }
}
