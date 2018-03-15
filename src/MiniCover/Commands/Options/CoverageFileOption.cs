using Microsoft.Extensions.CommandLineUtils;
using System;

namespace MiniCover.Commands.Options
{
    public class CoverageFileOption : IMiniCoverOption<string>
    {
        protected const string DefaultValue = "./coverage.json";
        protected string CoverageFilePath;
        private bool _invalidated;
        private CommandOption _option;
        private string _value;
        public string Description => $"Coverage file name [default: {DefaultValue}]";
        public string OptionTemplate => "--coverage-file";
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

        public virtual void Invalidate()
        {
            var proposalCoverageFilePath = _option.Value();
            if (string.IsNullOrWhiteSpace(proposalCoverageFilePath))
                proposalCoverageFilePath = DefaultValue;

            CoverageFilePath = proposalCoverageFilePath;
            _value = CoverageFilePath;
            _invalidated = true;
        }
    }
}