using System.Collections.Generic;

namespace MiniCover.CommandLine.Options
{
    class ExcludeTestsPatternOption : FilesPatternOption
    {
        private static readonly string[] _defaultValue = new string[] { "**/bin/**/*.cs", "**/obj/**/*.cs" };
        private const string _template = "--exclude-tests";
        private static readonly string _description = $"Pattern to exclude source files [default: {string.Join(" ", _defaultValue)}]";

        public ExcludeTestsPatternOption() 
            : base(_template, _description)
        {
        }

        protected override IList<string> GetDefaultValue()
        {
            return _defaultValue;
        }
    }
}