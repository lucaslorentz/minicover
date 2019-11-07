namespace MiniCover.CommandLine.Options
{
    class OpenCoverOutputOption : FileOption
    {
        private const string _defaultValue = "./opencovercoverage.xml";
        private const string _template = "--output";
        private static readonly string _description = $"Output file for OpenCover report [default: {_defaultValue}]";

        public OpenCoverOutputOption()
            : base(_template, _description)
        {
        }

        protected override string GetDefaultValue()
        {
            return _defaultValue;
        }
    }
}