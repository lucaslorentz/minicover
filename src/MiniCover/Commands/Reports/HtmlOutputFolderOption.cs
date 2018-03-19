using System.IO;
using MiniCover.Commands.Options;

namespace MiniCover.Commands.Reports
{
    internal class HtmlOutputFolderOption : MiniCoverOption<DirectoryInfo>
    {
        private const string DefaultValue = "./coverage-html";
        protected override string Description => $"Output folder for html report [default: {DefaultValue}]";
        protected override string OptionTemplate => "--output";

        protected override DirectoryInfo GetOptionValue()
        {
            var workingDirectoryPath = Option.Value() ?? DefaultValue;
            Directory.CreateDirectory(workingDirectoryPath);

            var workingDirectory = new DirectoryInfo(workingDirectoryPath);
            Validated = true;
            return workingDirectory;
        }
    }
}