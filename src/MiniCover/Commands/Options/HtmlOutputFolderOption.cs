using System.IO;

namespace MiniCover.Commands.Options
{
    internal class HtmlOutputFolderOption : MiniCoverOption<DirectoryInfo>
    {
        private const string DefaultValue = "./coverage-html";
        public override string Description => $"Output folder for html report [default: {DefaultValue}]";
        public override string OptionTemplate => "--output";

        public override void Invalidate()
        {
            var workingDirectoryPath = Option.Value() ?? DefaultValue;
            Directory.CreateDirectory(workingDirectoryPath);

            var workingDirectory = new DirectoryInfo(workingDirectoryPath);
            ValueField = workingDirectory;
            Invalidated = true;
        }
    }
}