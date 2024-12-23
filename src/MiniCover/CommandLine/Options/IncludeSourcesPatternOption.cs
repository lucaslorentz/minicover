using System.Collections.Generic;

namespace MiniCover.CommandLine.Options
{
    public class IncludeSourcesPatternOption : FilesPatternOption
    {
        public override string Name => "--sources";
        public override string Description => $"Pattern to include source files [default: {string.Join(" ", DefaultValue)}]";
        protected override IList<string> DefaultValue => new string[] { "src/**/*.cs" };
    }
}