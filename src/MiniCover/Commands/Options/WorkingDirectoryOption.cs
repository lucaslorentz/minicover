using System;
using System.IO;

namespace MiniCover.Commands.Options
{
    internal class WorkingDirectoryOption : MiniCoverOption<DirectoryInfo>
    {
        private const string DefaultValue = "./";
        private const string OptionTemplate = "--workdir";

        private static readonly string Description = $"Change working directory [default: {DefaultValue}]";

        internal WorkingDirectoryOption()
            : base(Description, OptionTemplate)
        {
        }

        protected override DirectoryInfo GetOptionValue()
        {
            var workingDirectoryPath = Option.Value() ?? DefaultValue;
            var directory = Directory.CreateDirectory(workingDirectoryPath);
            Console.WriteLine($"Changing working directory to '{directory.FullName}'");
            Directory.SetCurrentDirectory(directory.FullName);
            return directory;
        }

        protected override bool Validation() => true;
    }
}