using System.Collections.Generic;
using Microsoft.Extensions.CommandLineUtils;

namespace MiniCover.CommandLine
{
    abstract class MultiValueOption<TValue> : BaseOption<TValue>
    {
        protected MultiValueOption(string template, string description)
            : base(template, description, CommandOptionType.MultipleValue)
        {
        }

        protected sealed override TValue PrepareValue(CommandOption option)
        {
            return PrepareValue(option.Values);
        }

        protected abstract TValue PrepareValue(IList<string> values);
    }
}