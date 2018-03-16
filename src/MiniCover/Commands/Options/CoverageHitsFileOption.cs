namespace MiniCover.Commands.Options
{
    public class CoverageHitsFileOption : MiniCoverOption<string>
    {
        private const string DefaultValue = "coverage-hits.txt";
        public override string Description => $"Hits file name pattern [default: {DefaultValue}]";
        public override string OptionTemplate => "--hits-file";

        public override void Invalidate()
        {
            var proposalValue = Option.Value();
            if (string.IsNullOrWhiteSpace(proposalValue))
                proposalValue = DefaultValue;

            Invalidated = true;
            ValueField = proposalValue;
        }
    }
}