using System.IO;
using Microsoft.Extensions.Logging;

namespace MiniCover.CommandLine.Options
{
    class WorkingDirectoryOption : DirectoryOption
    {
        private const string _defaultValue = "./";
        private const string _template = "--workdir";
        private static readonly string _description = $"Change working directory [default: {_defaultValue}]";

        private readonly ILogger<WorkingDirectoryOption> _logger;

        public WorkingDirectoryOption(ILogger<WorkingDirectoryOption> logger)
            : base(_template, _description)
        {
            _logger = logger;
        }

        protected override DirectoryInfo PrepareValue(string value)
        {
            var directoryInfo = base.PrepareValue(value);
            var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
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