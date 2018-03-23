namespace MiniCover.Commands.Options
{
    internal class IncludeSourcesPatternOption : FilesPatternOption
    {
        protected override string DefaultValue => "**/*.cs";
        protected override string Description => $"Pattern to include source files [default: {DefaultValue}]";
        protected override string OptionTemplate => "--sources";
    }
}