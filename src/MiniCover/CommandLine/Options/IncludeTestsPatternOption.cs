using System.Collections.Generic;

namespace MiniCover.CommandLine.Options
{
    class IncludeTestsPatternOption : FilesPatternOption
    {
        private const string _defaultValue = "tests/**/*.cs";
        private const string _template = "--tests";
        private static readonly string _description = $"Pattern to include test files [default: {_defaultValue}]";

        public IncludeTestsPatternOption()
            : base(_template, _description)
        {
        }

        protected override IList<string> GetDefaultValue()
        {
            return new[] { _defaultValue };
        }
    }
}