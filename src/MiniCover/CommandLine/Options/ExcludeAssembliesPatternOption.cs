using System.Collections.Generic;

namespace MiniCover.CommandLine.Options
{
    class ExcludeAssembliesPatternOption : FilesPatternOption
    {
        private const string _template = "--exclude-assemblies";
        private const string _description = "Pattern to exclude assemblies";

        public ExcludeAssembliesPatternOption()
            : base(_template, _description)
        {
        }

        protected override IList<string> GetDefaultValue()
        {
            return new string[0];
        }
    }
}