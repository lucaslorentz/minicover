using System.IO.Abstractions;
using System.Threading.Tasks;
using FluentAssertions;
using MiniCover.CommandLine.Commands;
using MiniCover.CommandLine.Options;
using MiniCover.Core.Model;
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
        private readonly Mock<INoFailOption> _noFailOption;
        private readonly Mock<IHtmlReport> _htmlReport;

        public HtmlReportCommandTests()
        {
            _workingDirectoryOption = MockFor<IWorkingDirectoryOption>();
            _coverageLoadedFileOption = MockFor<ICoverageLoadedFileOption>();
            _htmlOutputDirectoryOption = MockFor<IHtmlOutputDirectoryOption>();
            _thresholdOption = MockFor<IThresholdOption>();
            _noFailOption = MockFor<INoFailOption>();
            _htmlReport = MockFor<IHtmlReport>();

            Sut = new HtmlReportCommand(
                _workingDirectoryOption.Object,
                _coverageLoadedFileOption.Object,
                _htmlOutputDirectoryOption.Object,
                _thresholdOption.Object,
                _noFailOption.Object,
                _htmlReport.Object
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
            _htmlOutputDirectoryOption.SetupGet(x => x.DirectoryInfo).Returns(output.Object);
            _thresholdOption.SetupGet(x => x.Value).Returns(threshold);
            _noFailOption.SetupGet(x => x.Value).Returns(noFail);
            _htmlReport.Setup(x => x.Execute(result, output.Object, threshold, noFail))
                .Returns(0);

            var exitCode = await Sut.Execute();
            exitCode.Should().Be(0);
        }
    }
}
