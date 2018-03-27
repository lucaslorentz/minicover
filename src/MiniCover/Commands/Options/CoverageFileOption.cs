namespace MiniCover.Commands.Options
{
    internal class CoverageFileOption : PathOption
    {
        private const string DefaultValue = "./coverage.json";
        private const string OptionTemplate = "--coverage-file";

        private static readonly string Description = $"Coverage file name [default: {DefaultValue}]";

        internal CoverageFileOption() : base(DefaultValue, Description, OptionTemplate)
        {
        }
    }
}