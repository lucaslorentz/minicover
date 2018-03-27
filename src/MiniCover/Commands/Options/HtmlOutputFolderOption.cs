using System.IO;

namespace MiniCover.Commands.Options
{
    internal class HtmlOutputFolderOption : MiniCoverOption<DirectoryInfo>
    {
        private const string DefaultValue = "./coverage-html";
        private const string OptionTemplate = "--output";

        private static readonly string Description = $"Output folder for html report [default: {DefaultValue}]";

        internal HtmlOutputFolderOption()
            : base(Description, OptionTemplate)
        {
        }

        protected override DirectoryInfo GetOptionValue()
        {
            var workingDirectoryPath = Option.Value() ?? DefaultValue;
            var workingDirectory = Directory.CreateDirectory(workingDirectoryPath);

            return workingDirectory;
        }

        protected override bool Validation() => true;
    }
}