namespace MiniCover.CommandLine.Options
{
    class HtmlOutputFolderOption : DirectoryOption
    {
        private const string _defaultValue = "./coverage-html";
        private const string _template = "--output";
        private static readonly string _description = $"Output folder for html report [default: {_defaultValue}]";

        public HtmlOutputFolderOption()
            : base(_template, _description)
        {
        }

        protected override string GetDefaultValue()
        {
            return _defaultValue;
        }
    }
}