using System.Collections.Generic;

namespace MiniCover.CommandLine.Options
{
    public class IncludeAssembliesPatternOption : FilesPatternOption
    {
        public override string Name => "--assemblies";
        public override string Description => $"Pattern to include assemblies [default: {string.Join(" ", DefaultValue)}]";
        protected override IList<string> DefaultValue => new string[] { "**/*.dll" };
    }
}