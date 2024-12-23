using System.IO.Abstractions;
using Microsoft.Extensions.Logging;

namespace MiniCover.CommandLine.Options
{
    public class WorkingDirectoryOption : DirectoryOption, IWorkingDirectoryOption
    {
        private readonly ILogger<WorkingDirectoryOption> _logger;
        private readonly IFileSystem _fileSystem;

        public WorkingDirectoryOption(
            ILogger<WorkingDirectoryOption> logger,
            IFileSystem fileSystem)
            : base(fileSystem)
        {
            _logger = logger;
            _fileSystem = fileSystem;
        }

        public override string Name => "--workdir";
        public override string Description => $"Change working directory [default: {DefaultValue}]";
        protected override string DefaultValue => "./";

        public override void ReceiveValue(string value)
        {
            base.ReceiveValue(value);

            var currentDirectory = _fileSystem.DirectoryInfo.New(_fileSystem.Directory.GetCurrentDirectory());
            if (DirectoryInfo.FullName != currentDirectory.FullName)
            {
                DirectoryInfo.Create();
                _logger.LogInformation("Changing working directory to {directory}", DirectoryInfo.FullName);
                _fileSystem.Directory.SetCurrentDirectory(DirectoryInfo.FullName);
            }
        }
    }
}