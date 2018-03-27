namespace MiniCover.Commands.Options
{
    internal class IncludeAssembliesPatternOption : FilesPatternOption
    {
        private const string DefaultValue = "**/*.dll";
        private const string OptionTemplate = "--assemblies";

        private static readonly string Description = $"Pattern to include assemblies [default: {DefaultValue}]";

        internal IncludeAssembliesPatternOption()
            : base(DefaultValue, Description, OptionTemplate)
        {
        }
    }
}