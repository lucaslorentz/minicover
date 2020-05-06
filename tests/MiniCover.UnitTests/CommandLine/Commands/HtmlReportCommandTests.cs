using System.IO.Abstractions;
using System.Threading.Tasks;
using FluentAssertions;
using MiniCover.CommandLine.Commands;
using MiniCover.CommandLine.Options;
using MiniCover.Model;
using MiniCover.Reports.Html;
using Moq;
using Xunit;

namespace MiniCover.UnitTests.CommandLine.Commands
{
    public class HtmlReportCommandTests : CommandTests<HtmlReportCommand>
    {
        private readonly Mock<IWorkingDirectoryOption> _workingDirectoryOption;
        private readonly Mock<ICoverageLoadedFileOption> _coverageLoadedFileOption;
        private readonly Mock<IHtmlOutputDirectoryOption> _htmlOutputDirectoryOption;
        private readonly Mock<IThresholdOption> _thresholdOption;
        private readonly Mock<IHtmlReport> _htmlReport;

        public HtmlReportCommandTests()
        {
            _workingDirectoryOption = MockFor<IWorkingDirectoryOption>();
            _coverageLoadedFileOption = MockFor<ICoverageLoadedFileOption>();
            _htmlOutputDirectoryOption = MockFor<IHtmlOutputDirectoryOption>();
            _thresholdOption = MockFor<IThresholdOption>();
            _htmlReport = MockFor<IHtmlReport>();

            Sut = new HtmlReportCommand(
                _workingDirectoryOption.Object,
                _coverageLoadedFileOption.Object,
                _htmlOutputDirectoryOption.Object,
                _thresholdOption.Object,
                _htmlReport.Object
            );
        }

        [Fact]
        public async Task Execute()
        {
            var result = new InstrumentationResult();
            var output = MockFor<IDirectoryInfo>();
            var threshold = 50f;

            _coverageLoadedFileOption.SetupGet(x => x.Result).Returns(result);
            _htmlOutputDirectoryOption.SetupGet(x => x.DirectoryInfo).Returns(output.Object);
            _thresholdOption.SetupGet(x => x.Value).Returns(threshold);
            _htmlReport.Setup(x => x.Execute(result, output.Object, threshold))
                .Returns(0);

            var exitCode = await Sut.Execute();
            exitCode.Should().Be(0);
        }
    }
}
