using System.IO.Abstractions;

namespace MiniCover.CommandLine.Options
{
    public class HitsDirectoryOption : DirectoryOption
    {
        public HitsDirectoryOption(IFileSystem fileSystem)
            : base(fileSystem)
        {
        }

        public override string Name => "--hits-directory";
        public override string Description => $"Directory to store hits files [default: {DefaultValue}]";
        protected override string DefaultValue => "./coverage-hits";
    }
}