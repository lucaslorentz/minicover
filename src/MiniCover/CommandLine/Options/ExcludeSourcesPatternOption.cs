using System.Collections.Generic;

namespace MiniCover.CommandLine.Options
{
    public class ExcludeSourcesPatternOption : FilesPatternOption
    {
        public override string Template => "--exclude-sources";
        public override string Description => $"Pattern to exclude source files [default: {string.Join(" ", DefaultValue)}]";
        protected override IList<string> DefaultValue => new string[] { "**/bin/**/*.cs", "**/obj/**/*.cs" };
    }
}