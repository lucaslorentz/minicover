namespace MiniCover.CommandLine.Options
{
    class CoverageFileOption :  FileOption
    {
        private const string _defaultValue = "./coverage.json";
        private const string _template = "--coverage-file";
        private static readonly string _description = $"Coverage file name [default: {_defaultValue}]";

        public CoverageFileOption()
            : base(_template, _description)
        {
        }

        protected override string GetDefaultValue()
        {
            return _defaultValue;
        }
    }
}