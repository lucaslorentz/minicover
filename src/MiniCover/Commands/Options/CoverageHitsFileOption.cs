namespace MiniCover.Commands.Options
{
    internal class CoverageHitsFileOption : PathOption
    {
        protected override string DefaultValue => "./coverage-hits.txt";
        protected override string Description => $"Hits file name pattern [default: {DefaultValue}]";
        protected override string OptionTemplate => "--hits-file";
    }
}