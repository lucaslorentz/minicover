using System.Collections.Generic;

namespace MiniCover.CommandLine.Options
{
    class ExcludeSourcesPatternOption : FilesPatternOption
    {
        private const string _template = "--exclude-sources";
        private const string _description = "Pattern to exclude source files";

        public ExcludeSourcesPatternOption() 
            : base(_template, _description)
        {
        }

        protected override IList<string> GetDefaultValue()
        {
            return new string[0];
        }
    }
}