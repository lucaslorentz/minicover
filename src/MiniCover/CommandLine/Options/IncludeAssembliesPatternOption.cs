using System.Collections.Generic;

namespace MiniCover.CommandLine.Options
{
    class IncludeAssembliesPatternOption : FilesPatternOption
    {
        private const string _defaultValue = "tests/**/bin/**.dll";
        private const string _template = "--assemblies";
        private static readonly string _description = $"Pattern to include assemblies [default: {_defaultValue}]";

        public IncludeAssembliesPatternOption()
            : base(_template, _description)
        {
        }

        protected override IList<string> GetDefaultValue()
        {
            return new[] { _defaultValue };
        }
    }
}