namespace MiniCover.Commands.Options
{
    internal class ExcludeSourcesPatternOption : FilesPatternOption
    {
        private const string DefaultValue = null;
        private const string Description = "Pattern to exclude source files";
        private const string OptionTemplate = "--exclude-sources";

        internal ExcludeSourcesPatternOption() 
            : base(DefaultValue, Description, OptionTemplate)
        {
        }
    }
}