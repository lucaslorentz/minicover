namespace MiniCover.Commands.Options
{
    internal class ExcludeSourcesPatternOption : FilesPatternOption
    {
        protected override string DefaultValue => null;
        protected override string Description => "Pattern to exclude source files";
        protected override string OptionTemplate => "--exclude-sources";
    }
}