using System.IO.Abstractions;

namespace MiniCover.CommandLine.Options
{
    class HitsDirectoryOption : DirectoryOption
    {
        private const string _defaultValue = "./coverage-hits";
        private const string _template = "--hits-directory";
        private static readonly string _description = $"Directory to store hits files [default: {_defaultValue}]";

        public HitsDirectoryOption(
            IFileSystem fileSystem)
            : base(_template, _description, fileSystem)
        {
        }

        protected override string GetDefaultValue()
        {
            return _defaultValue;
        }
    }
}