using System.IO.Abstractions;
using System.Threading.Tasks;
using FluentAssertions;
using MiniCover.CommandLine.Commands;
using MiniCover.CommandLine.Options;
using MiniCover.Model;
using MiniCover.Reports.Clover;
using Moq;
using Xunit;

namespace MiniCover.UnitTests.CommandLine.Commands
{
    public class CloverReportCommandTests : CommandTests<CloverReportCommand>
    {
        private readonly Mock<IWorkingDirectoryOption> _workingDirectoryOption;
        private readonly Mock<ICoverageLoadedFileOption> _coverageLoadedFileOption;
        private readonly Mock<ICloverOutputOption> _cloverOutputOption;
        private readonly Mock<ICloverReport> _cloverReport;

        public CloverReportCommandTests()
        {
            _workingDirectoryOption = MockFor<IWorkingDirectoryOption>();
            _coverageLoadedFileOption = MockFor<ICoverageLoadedFileOption>();
            _cloverOutputOption = MockFor<ICloverOutputOption>();
            _cloverReport = MockFor<ICloverReport>();

            Sut = new CloverReportCommand(
                _workingDirectoryOption.Object,
                _coverageLoadedFileOption.Object,
                _cloverOutputOption.Object,
                _cloverReport.Object
            );
        }

        [Fact]
        public async Task Execute()
        {
            var result = new InstrumentationResult();
            var output = MockFor<IFileInfo>();

            _coverageLoadedFileOption.SetupGet(x => x.Result).Returns(result);
            _cloverOutputOption.SetupGet(x => x.FileInfo).Returns(output.Object);
            _cloverReport.Setup(x => x.Execute(result, output.Object));

            var exitCode = await Sut.Execute();
            exitCode.Should().Be(0);
        }
    }
}
