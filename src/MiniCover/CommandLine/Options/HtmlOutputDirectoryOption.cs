using System.IO.Abstractions;

namespace MiniCover.CommandLine.Options
{
    public class HtmlOutputDirectoryOption : DirectoryOption, IHtmlOutputDirectoryOption
    {
        public HtmlOutputDirectoryOption(IFileSystem fileSystem)
            : base(fileSystem)
        {
        }

        public override string Name => "--output";
        public override string Description => $"Output folder for html report [default: {DefaultValue}]";
        protected override string DefaultValue => "./coverage-html";
    }
}