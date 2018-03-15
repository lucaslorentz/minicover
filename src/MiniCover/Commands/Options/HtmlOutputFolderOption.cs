using Microsoft.Extensions.CommandLineUtils;
using System;
using System.IO;

namespace MiniCover.Commands.Options
{
    internal class HtmlOutputFolderOption : IMiniCoverOption<DirectoryInfo>
    {
        private const string DefaultValue = "./coverage-html";
        private bool _invalidated;
        private CommandOption _option;
        private DirectoryInfo _value;
        public string Description => $"Output folder for html report [default: {DefaultValue}]";
        public string OptionTemplate => "--output";
        public CommandOptionType Type => CommandOptionType.SingleValue;

        public DirectoryInfo Value
        {
            get
            {
                if (_invalidated) return _value;
                throw new MemberAccessException("Option should be invalidate before Value access");
            }

            set => _value = value;
        }

        public void Initialize(CommandLineApplication commandContext)
        {
            _option = commandContext.Option(OptionTemplate, Description, Type);
        }

        public void Invalidate()
        {
            var workingDirectoryPath = _option.Value() ?? DefaultValue;
            Directory.CreateDirectory(workingDirectoryPath);

            var workingDirectory = new DirectoryInfo(workingDirectoryPath);
            _value = workingDirectory;
            _invalidated = true;
        }
    }
}