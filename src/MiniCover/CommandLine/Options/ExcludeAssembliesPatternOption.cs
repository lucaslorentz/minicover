using System.Collections.Generic;

namespace MiniCover.CommandLine.Options
{
    public class ExcludeAssembliesPatternOption : FilesPatternOption
    {
        public override string Template => "--exclude-assemblies";
        public override string Description => $"Pattern to exclude assemblies [default: {string.Join(" ", DefaultValue)}]";
        protected override IList<string> DefaultValue => new string[] { "**/obj/**/*.dll" };
    }
}