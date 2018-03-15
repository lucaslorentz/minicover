using Microsoft.Extensions.CommandLineUtils;
using System;
using System.IO;

namespace MiniCover.Commands.Options
{
    internal class WorkingDirectoryOption : IMiniCoverOption
    {
        private const string DefaultValue = "./";
        private CommandOption _option;
        public string Description => $"Change working directory [default: {DefaultValue}]";
        public string OptionTemplate => "--workdir";
        public CommandOptionType Type => CommandOptionType.SingleValue;

        public void Initialize(CommandLineApplication commandContext)
        {
            _option = commandContext.Option(OptionTemplate, Description, Type);
        }

        public void Invalidate()
        {
            var workingDirectoryPath = _option.Value() ?? DefaultValue;
            Directory.CreateDirectory(workingDirectoryPath);

            var workingDirectory = new DirectoryInfo(workingDirectoryPath);

            Console.WriteLine($"Changing working directory to '{workingDirectory.FullName}'");
            Directory.SetCurrentDirectory(workingDirectory.FullName);
        }
    }
}