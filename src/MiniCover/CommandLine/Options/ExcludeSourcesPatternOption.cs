using System.Collections.Generic;

namespace MiniCover.CommandLine.Options
{
    class ExcludeSourcesPatternOption : FilesPatternOption
    {
        private static readonly string[] _defaultValue = new string[] { "**/bin/**/*.cs", "**/obj/**/*.cs" };
        private const string _template = "--exclude-sources";
        private static readonly string _description = $"Pattern to exclude source files [default: {string.Join(" ", _defaultValue)}]";

        public ExcludeSourcesPatternOption() 
            : base(_template, _description)
        {
        }

        protected override IList<string> GetDefaultValue()
        {
            return _defaultValue;
        }
    }
}