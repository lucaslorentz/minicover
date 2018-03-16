namespace MiniCover.Commands.Options
{
    public class CoverageFileOption : MiniCoverOption<string>
    {
        protected const string DefaultValue = "./coverage.json";
        protected string CoverageFilePath;
        public override string Description => $"Coverage file name [default: {DefaultValue}]";
        public override string OptionTemplate => "--coverage-file";

        public override void Invalidate()
        {
            var proposalCoverageFilePath = Option.Value();
            if (string.IsNullOrWhiteSpace(proposalCoverageFilePath))
                proposalCoverageFilePath = DefaultValue;

            CoverageFilePath = proposalCoverageFilePath;
            ValueField = CoverageFilePath;
            Invalidated = true;
        }
    }
}