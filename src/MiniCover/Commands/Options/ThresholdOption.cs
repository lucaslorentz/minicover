using Microsoft.Extensions.CommandLineUtils;
using System;

namespace MiniCover.Commands.Options
{
    public class ThresholdOption : IMiniCoverOption<float>
    {
        protected const float DefaultValue = 90;
        protected string CoverageFilePath;
        private bool _invalidated;
        private CommandOption _option;
        private float _value;
        public string Description => $"Coverage percentage threshold [default: {DefaultValue}]";
        public string OptionTemplate => "--threshold";
        public CommandOptionType Type => CommandOptionType.SingleValue;

        public float Value
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
            if (!float.TryParse(_option.Value(), out var proposalThreshold))
            {
                proposalThreshold = DefaultValue;
            }

            _value = proposalThreshold / 100;
            _invalidated = true;
        }
    }
}