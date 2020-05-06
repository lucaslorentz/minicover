using System.IO.Abstractions;
using System.Threading.Tasks;
using MiniCover.CommandLine.Options;
using MiniCover.Reports.Coveralls;

namespace MiniCover.CommandLine.Commands
{
    public class CoverallsReportCommand : ICommand
    {
        public StringOption RootPathOption { get; } = new StringOption("--root-path", "Set the git root path");
        public StringOption OutputOption { get; } = new StringOption("--output", "Output file for coveralls report");
        public StringOption ServiceJobIdOption { get; } = new StringOption("--service-job-id", "Define service_job_id in coveralls json");
        public StringOption ServiceNameOption { get; } = new StringOption("--service-name", "Define service_name in coveralls json");
        public StringOption RepoTokenOption { get; } = new StringOption("--repo-token", "set the repo token");
        public StringOption CommitOption { get; } = new StringOption("--commit", "set the git commit id");
        public StringOption CommitMessageOption { get; } = new StringOption("--commit-message", "set the commit message");
        public StringOption CommitAuthorNameOption { get; } = new StringOption("--commit-author-name", "set the commit author name");
        public StringOption CommitAuthorEmailOption { get; } = new StringOption("--commit-author-email", "set the commit author email");
        public StringOption CommitCommitterNameOption { get; } = new StringOption("--commit-committer-name", "set the commit committer name");
        public StringOption CommitCommitterEmailOption { get; } = new StringOption("--commit-committer-email", "set the commit committer email");
        public StringOption BranchOption { get; } = new StringOption("--branch", "set the git branch");
        public StringOption RemoteOption { get; } = new StringOption("--remote", "set the git remote name");
        public StringOption RemoteUrlOption { get; } = new StringOption("--remote-url", "set the git remote url");

        private readonly IFileSystem _fileSystem;
        private readonly IWorkingDirectoryOption _workingDirectoryOption;
        private readonly ICoverageLoadedFileOption _coverageLoadedFileOption;
        private readonly ICoverallsReport _coverallsReport;

        public CoverallsReportCommand(
            IFileSystem fileSystem,
            IWorkingDirectoryOption workingDirectoryOption,
            ICoverageLoadedFileOption coverageLoadedFileOption,
            ICoverallsReport coverallsReport)
        {
            _fileSystem = fileSystem;
            _workingDirectoryOption = workingDirectoryOption;
            _coverageLoadedFileOption = coverageLoadedFileOption;
            _coverallsReport = coverallsReport;
        }

        public string CommandName => "coverallsreport";
        public string CommandDescription => "Write a coveralls-formatted JSON report to folder";

        public IOption[] Options => new IOption[]
        {
            _workingDirectoryOption,
            _coverageLoadedFileOption,
            RootPathOption,
            OutputOption,
            ServiceJobIdOption,
            ServiceNameOption,
            RepoTokenOption,
            CommitOption,
            CommitMessageOption,
            CommitAuthorNameOption,
            CommitAuthorEmailOption,
            CommitCommitterNameOption,
            CommitCommitterEmailOption,
            BranchOption,
            RemoteOption,
            RemoteUrlOption
        };

        public async Task<int> Execute()
        {
            var result = _coverageLoadedFileOption.Result;

            var relativeRootPath = RootPathOption.Value;

            var rootPath = !string.IsNullOrEmpty(relativeRootPath)
                ? _fileSystem.Path.GetFullPath(relativeRootPath)
                : _fileSystem.Directory.GetCurrentDirectory();

            return await _coverallsReport.Execute(
                result,
                OutputOption.Value,
                RepoTokenOption.Value,
                ServiceJobIdOption.Value,
                ServiceNameOption.Value,
                CommitMessageOption.Value,
                rootPath,
                CommitOption.Value,
                CommitAuthorNameOption.Value,
                CommitAuthorEmailOption.Value,
                CommitCommitterNameOption.Value,
                CommitCommitterEmailOption.Value,
                BranchOption.Value,
                RemoteOption.Value,
                RemoteUrlOption.Value);
        }
    }
}
