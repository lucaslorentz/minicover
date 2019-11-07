namespace MiniCover.CommandLine
{
    class StringOption : SingleValueOption<string>
    {
        public StringOption(string template, string description) : base(template, description)
        {
        }

        protected override string PrepareValue(string value)
        {
            return value;
        }
    }
}