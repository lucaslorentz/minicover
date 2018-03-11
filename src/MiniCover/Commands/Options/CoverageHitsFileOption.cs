using Microsoft.Extensions.CommandLineUtils;
using System;

namespace MiniCover.Commands.Options
{
    public class CoverageHitsFileOption : IMiniCoverOption<string>
    {
        private const string DefaultValue = "coverage-hits.txt";
        private bool _invalidated;
        private CommandOption _option;
        private string _value;
        public string Description => $"Hits file name pattern [default: {DefaultValue}]";
        public string OptionTemplate => "--hits-file";
        public CommandOptionType Type => CommandOptionType.SingleValue;

        public string Value
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
            var proposalValue = _option.Value();
            if (string.IsNullOrEmpty(proposalValue))
                proposalValue = DefaultValue;

            _invalidated = true;
            _value = proposalValue;
        }
    }
}