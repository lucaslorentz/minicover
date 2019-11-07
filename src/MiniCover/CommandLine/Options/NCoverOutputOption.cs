namespace MiniCover.CommandLine.Options
{
    class NCoverOutputOption : FileOption
    {
        private const string _defaultValue = "./coverage.xml";
        private const string _template = "--output";
        private static readonly string _description = $"Output file for NCover report [default: {_defaultValue}]";

        public NCoverOutputOption()
            : base(_template, _description)
        {
        }

        protected override string GetDefaultValue()
        {
            return _defaultValue;
        }
    }
}