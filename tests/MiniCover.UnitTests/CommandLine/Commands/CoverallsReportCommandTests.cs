using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using FluentAssertions;
using MiniCover.CommandLine.Commands;
using MiniCover.CommandLine.Options;
using MiniCover.Core.Model;
using MiniCover.Reports.Coveralls;
using MiniCover.TestHelpers;
using Moq;
using Xunit;

namespace MiniCover.UnitTests.CommandLine.Commands
{
    public class CoverallsReportCommandTests : CommandTests<CoverallsReportCommand>
    {
        private readonly MockFileSystem _mockFileSystem;
        private readonly Mock<IWorkingDirectoryOption> _workingDirectoryOption;
        private readonly Mock<ICoverageLoadedFileOption> _coverageLoadedFileOption;
        private readonly Mock<ICoverallsReport> _coverallsReport;

        public CoverallsReportCommandTests()
        {
            _mockFileSystem = new MockFileSystem();
            _workingDirectoryOption = MockFor<IWorkingDirectoryOption>();
            _coverageLoadedFileOption = MockFor<ICoverageLoadedFileOption>();
            _coverallsReport = MockFor<ICoverallsReport>();

            Sut = new CoverallsReportCommand(
                _mockFileSystem,
                _workingDirectoryOption.Object,
                _coverageLoadedFileOption.Object,
                _coverallsReport.Object
            );
        }

        [InlineData("root-path", "/current-directory/root-path")]
        [InlineData(null, "/current-directory")]
        [InlineData("", "/current-directory")]
        [Theory]
        public async Task Execute(string rootPath, string expectedRootPath)
        {
            rootPath = rootPath?.ToOSPath();
            expectedRootPath = expectedRootPath?.ToOSPath();

            var result = new InstrumentationResult();

            _mockFileSystem.Directory.SetCurrentDirectory("/current-directory".ToOSPath());

            Sut.OutputOption.ReceiveValue("coveralls.json");
            Sut.RepoTokenOption.ReceiveValue("repo-token");
            Sut.ServiceJobIdOption.ReceiveValue("service-job-id");
            Sut.ServiceNameOption.ReceiveValue("service-name");
            Sut.CommitMessageOption.ReceiveValue("commit-message");
            Sut.RootPathOption.ReceiveValue(rootPath);
            Sut.CommitOption.ReceiveValue("commit");
            Sut.CommitAuthorNameOption.ReceiveValue("commit-author-name");
            Sut.CommitAuthorEmailOption.ReceiveValue("commit-author-email");
            Sut.CommitCommitterNameOption.ReceiveValue("commit-committer-name");
            Sut.CommitCommitterEmailOption.ReceiveValue("commit-committer-email");
            Sut.BranchOption.ReceiveValue("branch");
            Sut.RemoteOption.ReceiveValue("remote");
            Sut.RemoteUrlOption.ReceiveValue("remote-url");

            _coverageLoadedFileOption.SetupGet(x => x.Result).Returns(result);
            _coverallsReport.Setup(x => x.Execute(
                result,
                "coveralls.json",
                "repo-token",
                "service-job-id",
                "service-name",
                "commit-message",
                expectedRootPath,
                "commit",
                "commit-author-name",
                "commit-author-email",
                "commit-committer-name",
                "commit-committer-email",
                "branch",
                "remote",
                "remote-url"))
                .ReturnsAsync(0);

            var exitCode = await Sut.Execute();
            exitCode.Should().Be(0);
        }
    }
}
