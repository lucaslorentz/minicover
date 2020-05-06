using System.IO.Abstractions;
using System.Threading.Tasks;
using FluentAssertions;
using MiniCover.CommandLine.Commands;
using MiniCover.CommandLine.Options;
using MiniCover.Model;
using MiniCover.Reports.OpenCover;
using Moq;
using Xunit;

namespace MiniCover.UnitTests.CommandLine.Commands
{
    public class OpenCoverReportCommandTests : CommandTests<OpenCoverReportCommand>
    {
        private readonly Mock<IWorkingDirectoryOption> _workingDirectoryOption;
        private readonly Mock<IOpenCoverOutputOption> _openCoverOutputOption;
        private readonly Mock<ICoverageLoadedFileOption> _coverageLoadedFileOption;
        private readonly Mock<IOpenCoverReport> _openCoverReport;

        public OpenCoverReportCommandTests()
        {
            _workingDirectoryOption = MockFor<IWorkingDirectoryOption>();
            _openCoverOutputOption = MockFor<IOpenCoverOutputOption>();
            _coverageLoadedFileOption = MockFor<ICoverageLoadedFileOption>();
            _openCoverReport = MockFor<IOpenCoverReport>();

            Sut = new OpenCoverReportCommand(
                _workingDirectoryOption.Object,
                _coverageLoadedFileOption.Object,
                _openCoverOutputOption.Object,
                _openCoverReport.Object
            );
        }

        [Fact]
        public async Task Execute()
        {
            var result = new InstrumentationResult();
            var output = MockFor<IFileInfo>();

            _coverageLoadedFileOption.SetupGet(x => x.Result).Returns(result);
            _openCoverOutputOption.SetupGet(x => x.FileInfo).Returns(output.Object);
            _openCoverReport.Setup(x => x.Execute(result, output.Object));

            var exitCode = await Sut.Execute();
            exitCode.Should().Be(0);
        }
    }
}
