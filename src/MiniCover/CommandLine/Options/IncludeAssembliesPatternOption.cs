using System.Collections.Generic;

namespace MiniCover.CommandLine.Options
{
    class IncludeAssembliesPatternOption : FilesPatternOption
    {
        private static readonly string[] _defaultValue = new string[] { "**/*.dll" };
        private const string _template = "--assemblies";
        private static readonly string _description = $"Pattern to include assemblies [default: {string.Join(" ", _defaultValue)}]";

        public IncludeAssembliesPatternOption()
            : base(_template, _description)
        {
        }

        protected override IList<string> GetDefaultValue()
        {
            return _defaultValue;
        }
    }
}