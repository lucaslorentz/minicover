using System.Collections.Generic;

namespace MiniCover.CommandLine.Options
{
    public class IncludeTestsPatternOption : FilesPatternOption
    {
        public override string Template => "--tests";
        public override string Description => $"Pattern to include test files [default: {string.Join(" ", DefaultValue)}]";
        protected override IList<string> DefaultValue => new string[] { "tests/**/*.cs", "test/**/*.cs" };
    }
}