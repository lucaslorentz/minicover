namespace MiniCover.CommandLine.Options
{
    class CloverOutputOption :  FileOption
    {
        private const string _defaultValue = "./clover.xml";
        private const string _template = "--output";
        private static readonly string _description = $"Output file for Clover report [default: {_defaultValue}]";

        public CloverOutputOption()
            : base(_template, _description)
        {
        }

        protected override string GetDefaultValue()
        {
            return _defaultValue;
        }
    }
}