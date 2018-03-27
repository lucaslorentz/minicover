namespace MiniCover.Commands.Options
{
    internal class ExcludeAssembliesPatternOption : FilesPatternOption
    {
        private const string DefaultValue = null;
        private const string Description = "Pattern to exclude assemblies";
        private const string OptionTemplate = "--exclude-assemblies";

        public ExcludeAssembliesPatternOption()
            : base(DefaultValue, Description, OptionTemplate)
        {
        }
    }
}