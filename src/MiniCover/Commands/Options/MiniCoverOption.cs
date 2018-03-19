using Microsoft.Extensions.CommandLineUtils;
using System;

namespace MiniCover.Commands.Options
{
    public abstract class MiniCoverOption<TValue> : IMiniCoverOption<TValue>
    {
        private CommandOption option;
        private bool validated;
        private TValue value;

        protected abstract string Description { get; }
        protected abstract string OptionTemplate { get; }
        protected virtual CommandOptionType Type => CommandOptionType.SingleValue;

        public TValue Value
        {
            get
            {
                if (!validated)
                    throw new MemberAccessException("Option should be validated before Value access");

                return value;
            }
        }

        public virtual void AddTo(CommandLineApplication command)
        {
            option = command.Option(OptionTemplate, Description, Type);
        }

        public void Validate()
        {
            value = GetOptionValue(option);
            validated = true;
        }

        protected abstract TValue GetOptionValue(CommandOption option);
    }
}