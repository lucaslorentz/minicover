using System;
using System.IO;

namespace MiniCover.Commands.Options
{
    internal class WorkingDirectoryOption : MiniCoverOption<DirectoryInfo>
    {
        private const string DefaultValue = "./";
        protected override string Description => $"Change working directory [default: {DefaultValue}]";
        protected override string OptionTemplate => "--workdir";

        protected override DirectoryInfo GetOptionValue()
        {
            var workingDirectoryPath = Option.Value() ?? DefaultValue;
            Directory.CreateDirectory(workingDirectoryPath);
            var directory = new DirectoryInfo(workingDirectoryPath);
            Console.WriteLine($"Changing working directory to '{directory.FullName}'");
            Directory.SetCurrentDirectory(directory.FullName);
            return new DirectoryInfo(workingDirectoryPath);
        }
    }
}