using System.IO.Abstractions;

namespace MiniCover.CommandLine.Options
{
    public abstract class DirectoryOption : ISingleValueOption
    {
        private readonly IFileSystem _fileSystem;

        protected DirectoryOption(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public IDirectoryInfo DirectoryInfo { get; private set; }
        public abstract string Template { get; }
        public abstract string Description { get; }
        protected abstract string DefaultValue { get; }

        public virtual void ReceiveValue(string value)
        {
            DirectoryInfo = _fileSystem.DirectoryInfo.FromDirectoryName(value ?? DefaultValue);
        }
    }
}