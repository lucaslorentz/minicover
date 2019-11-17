using System.IO;
using System.Threading.Tasks;
using MiniCover.CommandLine.Options;
using MiniCover.Reports.Coveralls;

namespace MiniCover.CommandLine.Commands
{
    class CoverallsReportCommand : BaseCommand
    {
        private const string _name = "coverallsreport";
        private const string _description = "Write a coveralls-formatted JSON report to folder";

        private readonly StringOption _rootPathOption = new StringOption("--root-path", "Set the git root path");
        private readonly StringOption _outputOption = new StringOption("--output", "Output file for coveralls report");
        private readonly StringOption _serviceJobIdOption = new StringOption("--service-job-id", "Define service_job_id in coveralls json");
        private readonly StringOption _serviceNameOption = new StringOption("--service-name", "Define service_name in coveralls json");
        private readonly StringOption _repoTokenOption = new StringOption("--repo-token", "set the repo token");
        private readonly StringOption _commitOption = new StringOption("--commit", "set the git commit id");
        private readonly StringOption _commitMessageOption = new StringOption("--commit-message", "set the commit message");
        private readonly StringOption _commitAuthorNameOption = new StringOption("--commit-author-name", "set the commit author name");
        private readonly StringOption _commitAuthorEmailOption = new StringOption("--commit-author-email", "set the commit author email");
        private readonly StringOption _commitCommitterNameOption = new StringOption("--commit-committer-name", "set the commit committer name");
        private readonly StringOption _commitCommitterEmailOption = new StringOption("--commit-committer-email", "set the commit committer email");
        private readonly StringOption _branchOption = new StringOption("--branch", "set the git branch");
        private readonly StringOption _remoteOption = new StringOption("--remote", "set the git remote name");
        private readonly StringOption _remoteUrlOption = new StringOption("--remote-url", "set the git remote url");
        private readonly CoverageLoadedFileOption _coverageLoadedFileOption;

        public CoverallsReportCommand(
            WorkingDirectoryOption workingDirectoryOption,
            CoverageLoadedFileOption coverageLoadedFileOption)
            : base(_name, _description)
        {
            _coverageLoadedFileOption = coverageLoadedFileOption;

            Options = new IOption[]
            {
                workingDirectoryOption,
                coverageLoadedFileOption,
                _rootPathOption,
                _outputOption,
                _serviceJobIdOption,
                _serviceNameOption,
                _repoTokenOption,
                _commitOption,
                _commitMessageOption,
                _commitAuthorNameOption,
                _commitAuthorEmailOption,
                _commitCommitterNameOption,
                _commitCommitterEmailOption,
                _branchOption,
                _remoteOption,
                _remoteUrlOption
            };
        }

        protected override async Task<int> Execute()
        {
            var result = _coverageLoadedFileOption.Result;
            var output = _outputOption.Value;

            var relativeRootPath = _rootPathOption.Value;

            var rootPath = !string.IsNullOrEmpty(relativeRootPath)
                ? Path.GetFullPath(relativeRootPath)
                : Directory.GetCurrentDirectory();

            var report = new CoverallsReport(
                output,
                _repoTokenOption.Value,
                _serviceJobIdOption.Value,
                _serviceNameOption.Value,
                _commitMessageOption.Value,
                rootPath,
                _commitOption.Value,
                _commitAuthorNameOption.Value,
                _commitAuthorEmailOption.Value,
                _commitCommitterNameOption.Value,
                _commitCommitterEmailOption.Value,
                _branchOption.Value,
                _remoteOption.Value,
                _remoteUrlOption.Value
            );

            return await report.Execute(result);
        }
    }
}
