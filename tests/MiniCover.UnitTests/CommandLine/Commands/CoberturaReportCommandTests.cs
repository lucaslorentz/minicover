using System.IO.Abstractions;
using System.Threading.Tasks;
using FluentAssertions;
using MiniCover.CommandLine.Commands;
using MiniCover.CommandLine.Options;
using MiniCover.Core.Model;
using MiniCover.Reports.Cobertura;
using Moq;
using Xunit;

namespace MiniCover.UnitTests.CommandLine.Commands
{
    public class CoberturaReportCommandTests : CommandTests<CoberturaReportCommand>
    {
        private readonly Mock<IWorkingDirectoryOption> _workingDirectoryOption;
        private readonly Mock<ICoberturaOutputOption> _coberturaOutputOption;
        private readonly Mock<ICoverageLoadedFileOption> _coverageLoadedFileOption;
        private readonly Mock<ICoberturaReport> _coberturaReport;

        public CoberturaReportCommandTests()
        {
            _workingDirectoryOption = MockFor<IWorkingDirectoryOption>();
            _coberturaOutputOption = MockFor<ICoberturaOutputOption>();
            _coverageLoadedFileOption = MockFor<ICoverageLoadedFileOption>();
            _coberturaReport = MockFor<ICoberturaReport>();

            Sut = new CoberturaReportCommand(
                _workingDirectoryOption.Object,
                _coverageLoadedFileOption.Object,
                _coberturaOutputOption.Object,
                _coberturaReport.Object
            );
        }

        [Fact]
        public async Task Execute()
        {
            var result = new InstrumentationResult();
            var output = MockFor<IFileInfo>();

            _coverageLoadedFileOption.SetupGet(x => x.Result).Returns(result);
            _coberturaOutputOption.SetupGet(x => x.FileInfo).Returns(output.Object);
            _coberturaReport.Setup(x => x.Execute(result, output.Object));

            var exitCode = await Sut.Execute();
            exitCode.Should().Be(0);
        }
    }
}
