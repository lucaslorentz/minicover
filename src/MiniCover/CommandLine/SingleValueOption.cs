using Microsoft.Extensions.CommandLineUtils;

namespace MiniCover.CommandLine
{
    abstract class SingleValueOption<TValue> : BaseOption<TValue>
    {
        protected SingleValueOption(string template, string description)
            : base(template, description, CommandOptionType.SingleValue)
        {
        }

        protected sealed override TValue PrepareValue(CommandOption option)
        {
            return PrepareValue(option.HasValue() ? option.Value() : null);
        }

        protected abstract TValue PrepareValue(string value);
    }
}