﻿using Microsoft.Extensions.CommandLineUtils;
using System;

namespace MiniCover.Commands.Options
{
    public abstract class MiniCoverOption<TValue> : IMiniCoverOption<TValue>
    {
        protected CommandOption Option;
        protected bool Validated;
        protected TValue ValueField { get; private set; }

        public TValue Value
        {
            get
            {
                if (!Validated)
                    throw new MemberAccessException("Option should be validated before Value access");

                return ValueField;
            }
        }

        protected abstract string Description { get; }
        protected abstract string OptionTemplate { get; }
        protected virtual CommandOptionType Type => CommandOptionType.SingleValue;

        public virtual void AddTo(CommandLineApplication command)
        {
            Option = command.Option(OptionTemplate, Description, Type);
        }

        public virtual void Validate()
        {
            ValueField = GetOptionValue();
            Validated = true;
        }

        protected abstract TValue GetOptionValue();
    }
}