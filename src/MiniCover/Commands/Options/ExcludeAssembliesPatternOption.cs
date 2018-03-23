namespace MiniCover.Commands.Options
{
    internal class ExcludeAssembliesPatternOption : FilesPatternOption
    {
        protected override string DefaultValue => null;
        protected override string Description => "Pattern to exclude assemblies";
        protected override string OptionTemplate => "--exclude-assemblies";
    }
}