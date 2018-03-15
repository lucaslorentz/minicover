using Microsoft.Extensions.CommandLineUtils;
using System;

namespace MiniCover.Commands.Options
{
    internal class NCoverOutputOption : IMiniCoverOption<string>
    {
        private const string DefaultValue = "./coverage.xml";
        private bool _invalidated;
        private CommandOption _option;
        private string _value;
        public string Description => $"Output file for NCover report [default: {DefaultValue}]";
        public string OptionTemplate => "--output";
        public CommandOptionType Type => CommandOptionType.SingleValue;

        public string Value
        {
            get
            {
                if (_invalidated) return _value;
                throw new MemberAccessException("Option should be invalidate before Value access");
            }
        }

        public void Initialize(CommandLineApplication commandContext)
        {
            _option = commandContext.Option(OptionTemplate, Description, Type);
        }

        public void Invalidate()
        {
            var coverageFilePath = _option.Value() ?? DefaultValue;
            _value = new TouchFile(coverageFilePath).FullName;
            _invalidated = true;
        }
    }
}