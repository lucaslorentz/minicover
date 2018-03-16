namespace MiniCover.Commands.Options
{
    public class ThresholdOption : MiniCoverOption<float>
    {
        protected const float DefaultValue = 90;
        protected string CoverageFilePath;
        public override string Description => $"Coverage percentage threshold [default: {DefaultValue}]";
        public override string OptionTemplate => "--threshold";

        public override void Invalidate()
        {
            if (!float.TryParse(Option.Value(), out var proposalThreshold))
            {
                proposalThreshold = DefaultValue;
            }

            ValueField = proposalThreshold / 100;
            Invalidated = true;
        }
    }
}