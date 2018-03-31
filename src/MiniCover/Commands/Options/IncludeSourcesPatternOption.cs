namespace MiniCover.Commands.Options
{
    internal class IncludeSourcesPatternOption : FilesPatternOption
    {
        private const string DefaultValue = "**/*.cs";
        private const string OptionTemplate = "--sources";

        private static readonly string Description = $"Pattern to include source files [default: {DefaultValue}]";

        internal IncludeSourcesPatternOption()
                    : base(DefaultValue, Description, OptionTemplate)
        {
        }
    }
}