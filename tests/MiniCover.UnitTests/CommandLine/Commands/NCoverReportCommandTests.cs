using System.IO.Abstractions;
using System.Threading.Tasks;
using FluentAssertions;
using MiniCover.CommandLine.Commands;
using MiniCover.CommandLine.Options;
using MiniCover.Model;
using MiniCover.Reports.NCover;
using Moq;
using Xunit;

namespace MiniCover.UnitTests.CommandLine.Commands
{
    public class NCoverReportCommandTests : CommandTests<NCoverReportCommand>
    {
        private readonly Mock<IWorkingDirectoryOption> _workingDirectoryOption;
        private readonly Mock<INCoverOutputOption> _nCoverOutputOption;
        private readonly Mock<ICoverageLoadedFileOption> _coverageLoadedFileOption;
        private readonly Mock<INCoverReport> _nCoverReport;

        public NCoverReportCommandTests()
        {
            _workingDirectoryOption = MockFor<IWorkingDirectoryOption>();
            _nCoverOutputOption = MockFor<INCoverOutputOption>();
            _coverageLoadedFileOption = MockFor<ICoverageLoadedFileOption>();
            _nCoverReport = MockFor<INCoverReport>();

            Sut = new NCoverReportCommand(
                _workingDirectoryOption.Object,
                _coverageLoadedFileOption.Object,
                _nCoverOutputOption.Object,
                _nCoverReport.Object
            );
        }

        [Fact]
        public async Task Execute()
        {
            var result = new InstrumentationResult();
            var output = MockFor<IFileInfo>();

            _coverageLoadedFileOption.SetupGet(x => x.Result).Returns(result);
            _nCoverOutputOption.SetupGet(x => x.FileInfo).Returns(output.Object);
            _nCoverReport.Setup(x => x.Execute(result, output.Object));

            var exitCode = await Sut.Execute();
            exitCode.Should().Be(0);
        }
    }
}
