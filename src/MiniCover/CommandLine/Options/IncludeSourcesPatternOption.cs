using System.Collections.Generic;

namespace MiniCover.CommandLine.Options
{
    class IncludeSourcesPatternOption : FilesPatternOption
    {
        private static readonly string[] _defaultValue = new string[] { "src/**/*.cs" };
        private const string _template = "--sources";
        private static readonly string _description = $"Pattern to include source files [default: {string.Join(" ", _defaultValue)}]";

        public IncludeSourcesPatternOption()
            : base(_template, _description)
        {
        }

        protected override IList<string> GetDefaultValue()
        {
            return _defaultValue;
        }
    }
}