using System.Collections.Generic;

namespace MiniCover.CommandLine.Options
{
    class ExcludeTestsPatternOption : FilesPatternOption
    {
        private const string _template = "--exclude-tests";
        private const string _description = "Pattern to exclude source files";

        public ExcludeTestsPatternOption() 
            : base(_template, _description)
        {
        }

        protected override IList<string> GetDefaultValue()
        {
            return new string[0];
        }
    }
}