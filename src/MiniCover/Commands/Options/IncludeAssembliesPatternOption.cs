namespace MiniCover.Commands.Options
{
    internal class IncludeAssembliesPatternOption : FilesPatternOption
    {
        protected override string DefaultValue => "**/*.dll";
        protected override string Description => $"Pattern to include assemblies [default: {DefaultValue}]";
        protected override string OptionTemplate => "--assemblies";
    }
}