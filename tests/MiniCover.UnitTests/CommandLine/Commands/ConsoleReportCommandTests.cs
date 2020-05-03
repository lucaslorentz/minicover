using System.IO.Abstractions;
using System.Threading.Tasks;
using FluentAssertions;
using MiniCover.CommandLine.Commands;
using MiniCover.CommandLine.Options;
using MiniCover.Model;
using MiniCover.Reports;
using Moq;
using Xunit;

namespace MiniCover.UnitTests.CommandLine.Commands
{
    public class ConsoleReportCommandTests : CommandTests<ConsoleReportCommand>
    {
        private readonly Mock<IWorkingDirectoryOption> _workingDirectoryOption;
        private readonly Mock<ICoverageLoadedFileOption> _coverageLoadedFileOption;
        private readonly Mock<IThresholdOption> _thresholdOption;
        private readonly Mock<IConsoleReport> _consoleReport;

        public ConsoleReportCommandTests()
        {
            _workingDirectoryOption = MockFor<IWorkingDirectoryOption>();
            _coverageLoadedFileOption = MockFor<ICoverageLoadedFileOption>();
            _thresholdOption = MockFor<IThresholdOption>();
            _consoleReport = MockFor<IConsoleReport>();

            Sut = new ConsoleReportCommand(
                _workingDirectoryOption.Object,
                _coverageLoadedFileOption.Object,
                _thresholdOption.Object,
                _consoleReport.Object
            );
        }

        [Fact]
        public async Task Execute()
        {
            var result = new InstrumentationResult();
            var output = MockFor<IDirectoryInfo>();
            var threshold = 50f;

            _coverageLoadedFileOption.SetupGet(x => x.Result).Returns(result);
            _thresholdOption.SetupGet(x => x.Value).Returns(threshold);
            _consoleReport.Setup(x => x.Execute(result, threshold))
                .Returns(0);

            var exitCode = await Sut.Execute();
            exitCode.Should().Be(0);
        }
    }
}
