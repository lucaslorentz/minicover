namespace MiniCover.Commands.Options
{
    internal class CoverageFileOption : PathOption
    {
        protected override string DefaultValue => "./coverage.json";
        protected override string Description => $"Coverage file name [default: {DefaultValue}]";
        protected override string OptionTemplate => "--coverage-file";
    }
}