using Microsoft.Extensions.CommandLineUtils;
using System;

namespace MiniCover.Commands.Options
{
    public abstract class MiniCoverOption<TValue> : IMiniCoverOption<TValue>
    {
        protected CommandOption Option;

        private readonly string _description;
        private readonly string _optionTemplate;
        private readonly CommandOptionType _type;

        protected MiniCoverOption(string description, string optionTemplate, CommandOptionType type = CommandOptionType.SingleValue)
        {
            _description = description;
            _optionTemplate = optionTemplate;
            _type = type;
        }

        protected bool Validated { get; private set; }
        protected TValue ValueField { get; private set; }

        public virtual void AddTo(CommandLineApplication command)
        {
            Option = command.Option(_optionTemplate, _description, _type);
        }

        public TValue GetValue()
        {
            if (!Validated)
                throw new MemberAccessException("Option should be validated before GetValue access");

            return ValueField;
        }

        public void Validate()
        {
            ValueField = GetOptionValue();
            Validated = Validation();
        }

        protected abstract TValue GetOptionValue();

        protected abstract bool Validation();
    }
}