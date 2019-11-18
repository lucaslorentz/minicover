using System.Collections.Generic;

namespace MiniCover.CommandLine.Options
{
    class IncludeTestsPatternOption : FilesPatternOption
    {
        private static readonly string[] _defaultValue = new string[] { "tests/**/*.cs", "test/**/*.cs" };
        private const string _template = "--tests";
        private static readonly string _description = $"Pattern to include test files [default: {string.Join(" ", _defaultValue)}]";

        public IncludeTestsPatternOption()
            : base(_template, _description)
        {
        }

        protected override IList<string> GetDefaultValue()
        {
            return _defaultValue;
        }
    }
}