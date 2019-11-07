using Microsoft.Extensions.CommandLineUtils;

namespace MiniCover.CommandLine
{
    public abstract class BaseOption<TValue> : IOption<TValue>
    {
        private readonly string _template;
        private readonly string _description;
        private readonly CommandOptionType _type;

        private CommandOption _option;

        protected BaseOption(string template, string description, CommandOptionType type)
        {
            _template = template;
            _description = description;
            _type = type;
        }

        public TValue Value { get; private set; }

        public void AddTo(CommandLineApplication command)
        {
            _option = command.Option(_template, _description, _type);
        }

        public void Prepare()
        {
            Value = PrepareValue(_option);
        }

        protected abstract TValue PrepareValue(CommandOption option);
    }
}