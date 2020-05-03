using System.IO.Abstractions;

namespace MiniCover.CommandLine.Options
{
    public abstract class FileOption : ISingleValueOption
    {
        private readonly IFileSystem _fileSystem;

        protected FileOption(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public IFileInfo FileInfo { get; private set; }
        public abstract string Template { get; }
        public abstract string Description { get; }
        protected abstract string DefaultValue { get; }

        public virtual void ReceiveValue(string value)
        {
            FileInfo = _fileSystem.FileInfo.FromFileName(value ?? DefaultValue);
        }
    }
}