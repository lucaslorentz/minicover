using System.IO.Abstractions;

namespace MiniCover.CommandLine.Options
{
    class HtmlOutputFolderOption : DirectoryOption
    {
        private const string _defaultValue = "./coverage-html";
        private const string _template = "--output";
        private static readonly string _description = $"Output folder for html report [default: {_defaultValue}]";

        public HtmlOutputFolderOption(
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