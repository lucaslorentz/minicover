using System;
using System.IO;

namespace MiniCover.Commands.Options
{
    internal class WorkingDirectoryOption : MiniCoverOption<DirectoryInfo>
    {
        private const string DefaultValue = "./";
        private const string OptionTemplate = "--workdir";

        private static readonly string Description = $"Change working directory [default: {DefaultValue}]";

        public WorkingDirectoryOption()
            : base(Description, OptionTemplate)
        {
        }

        protected override DirectoryInfo GetOptionValue()
        {
            var workingDirectoryPath = Option.Value() ?? DefaultValue;
            Directory.CreateDirectory(workingDirectoryPath);
            var directory = new DirectoryInfo(workingDirectoryPath);
            Console.WriteLine($"Changing working directory to '{directory.FullName}'");
            Directory.SetCurrentDirectory(directory.FullName);
            return new DirectoryInfo(workingDirectoryPath);
        }

        protected override bool Validation() => true;
    }
}