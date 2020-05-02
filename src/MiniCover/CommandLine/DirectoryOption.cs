using System.IO;
using System.IO.Abstractions;

namespace MiniCover.CommandLine.Options
{
    abstract class DirectoryOption : SingleValueOption<IDirectoryInfo>
    {
        private readonly IFileSystem _fileSystem;

        protected DirectoryOption(
            string template,
            string description,
            IFileSystem fileSystem)
            : base(template, description)
        {
            _fileSystem = fileSystem;
        }

        protected override IDirectoryInfo PrepareValue(string value)
        {
            return _fileSystem.DirectoryInfo.FromDirectoryName(value ?? GetDefaultValue());
        }

        protected abstract string GetDefaultValue();
    }
}