namespace MiniCover.CommandLine.Options
{
    class HitsDirectoryOption : DirectoryOption
    {
        private const string _defaultValue = "./coverage-hits";
        private const string _template = "--hits-directory";
        private static readonly string _description = $"Path to store hits files [default: {_defaultValue}]";

        public HitsDirectoryOption()
            : base(_template, _description)
        {
        }

        protected override string GetDefaultValue()
        {
            return _defaultValue;
        }
    }
}