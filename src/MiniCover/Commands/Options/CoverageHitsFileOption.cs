namespace MiniCover.Commands.Options
{
    internal class CoverageHitsFileOption : PathOption
    {
        private const string DefaultValue = "./coverage-hits.txt";
        private const string OptionTemplate = "--hits-file";

        private static readonly string Description = $"Hits file name pattern [default: {DefaultValue}]";

        public CoverageHitsFileOption()
            : base(DefaultValue, Description, OptionTemplate)
        {
        }
    }
}