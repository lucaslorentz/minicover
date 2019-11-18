using System.Collections.Generic;

namespace MiniCover.CommandLine.Options
{
    class ExcludeAssembliesPatternOption : FilesPatternOption
    {
        private static readonly string[] _defaultValue = new string[] { "**/obj/**/*.dll" };
        private const string _template = "--exclude-assemblies";
        private static readonly string _description = $"Pattern to exclude assemblies [default: {string.Join(" ", _defaultValue)}]";

        public ExcludeAssembliesPatternOption()
            : base(_template, _description)
        {
        }

        protected override IList<string> GetDefaultValue()
        {
            return _defaultValue;
        }
    }
}