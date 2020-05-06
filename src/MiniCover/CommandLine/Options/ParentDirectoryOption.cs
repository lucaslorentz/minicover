using System.IO.Abstractions;

namespace MiniCover.CommandLine.Options
{
    public class ParentDirectoryOption : DirectoryOption
    {
        public ParentDirectoryOption(IFileSystem fileSystem)
            : base(fileSystem)
        {
        }

        public override string Template => "--parentdir";
        public override string Description => "Set parent directory for assemblies and source directories (if not used, falls back to --workdir)";
        protected override string DefaultValue => "./";
    }
}