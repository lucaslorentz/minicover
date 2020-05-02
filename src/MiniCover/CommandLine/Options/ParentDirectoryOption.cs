using System.IO;
using System.IO.Abstractions;

namespace MiniCover.CommandLine.Options
{
    class ParentDirectoryOption : DirectoryOption
    {
        private const string _template = "--parentdir";
        private static readonly string _description = "Set parent directory for assemblies and source directories (if not used, falls back to --workdir)";

        public ParentDirectoryOption(
            IFileSystem fileSystem)
            : base(_template, _description, fileSystem)
        {
        }

        protected override string GetDefaultValue()
        {
            return Directory.GetCurrentDirectory();
        }
    }
}