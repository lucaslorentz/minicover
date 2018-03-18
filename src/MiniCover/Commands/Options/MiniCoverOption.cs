using Microsoft.Extensions.CommandLineUtils;
using System;

namespace MiniCover.Commands.Options
{
    public abstract class MiniCoverOption<TValue> : IMiniCoverOption<TValue>
    {
        protected bool Validated;
        protected TValue ValueField;
        protected abstract string Description { get; }
        protected abstract string OptionTemplate { get; }
        protected virtual CommandOptionType Type => CommandOptionType.SingleValue;

        public TValue Value
        {
            get
            {
                if (Validated) return ValueField;
                throw new MemberAccessException("Option should be invalidate before Value access");
            }
        }

        protected CommandOption Option { get; set; }

        public virtual void Initialize(CommandLineApplication commandContext)
        {
            Option = commandContext.Option(OptionTemplate, Description, Type);
        }

        public abstract void Validate();
    }
}