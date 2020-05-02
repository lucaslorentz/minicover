using System.IO;
using System.IO.Abstractions;
using Microsoft.Extensions.Logging;

namespace MiniCover.CommandLine.Options
{
    class WorkingDirectoryOption : DirectoryOption
    {
        private const string _defaultValue = "./";
        private const string _template = "--workdir";
        private static readonly string _description = $"Change working directory [default: {_defaultValue}]";

        private readonly ILogger<WorkingDirectoryOption> _logger;
        private readonly IFileSystem _fileSystem;

        public WorkingDirectoryOption(
            ILogger<WorkingDirectoryOption> logger,
            IFileSystem fileSystem)
            : base(_template, _description, fileSystem)
        {
            _logger = logger;
            _fileSystem = fileSystem;
        }

        protected override IDirectoryInfo PrepareValue(string value)
        {
            var directoryInfo = base.PrepareValue(value);
            var currentDirectory = _fileSystem.DirectoryInfo.FromDirectoryName(Directory.GetCurrentDirectory());
            if (directoryInfo.FullName != currentDirectory.FullName)
            {
                directoryInfo.Create();
                _logger.LogInformation("Changing working directory to {directory}", directoryInfo.FullName);
                Directory.SetCurrentDirectory(directoryInfo.FullName);
            }
            return directoryInfo;
        }

        protected override string GetDefaultValue()
        {
            return _defaultValue;
        }
    }
}