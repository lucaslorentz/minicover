namespace MiniCover.Commands.Options
{
    internal abstract class PathOption : MiniCoverOption<string>
    {
        protected abstract string DefaultValue { get; }

        protected override string GetOptionValue()
        {
            var proposalFilePath = Option.Value();
            if (string.IsNullOrWhiteSpace(proposalFilePath))
                proposalFilePath = DefaultValue;

            return proposalFilePath;
        }
    }

    internal class CoverageHitsFileOption : PathOption
    {
        protected override string DefaultValue => "./coverage-hits.txt";
        protected override string Description => $"Hits file name pattern [default: {DefaultValue}]";
        protected override string OptionTemplate => "--hits-file";
    }

    internal class CoverageFileOption : PathOption
    {
        protected override string DefaultValue => "./coverage.json";
        protected override string Description => $"Coverage file name [default: {DefaultValue}]";
        protected override string OptionTemplate => "--coverage-file";
    }
}