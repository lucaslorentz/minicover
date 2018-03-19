namespace MiniCover.Commands.Options
{
    internal class ThresholdOption : MiniCoverOption<float>
    {
        protected const float DefaultValue = 90;
        protected string CoverageFilePath;
        protected override string Description => $"Coverage percentage threshold [default: {DefaultValue}]";
        protected override string OptionTemplate => "--threshold";

        protected override float GetOptionValue()
        {
            if (!float.TryParse(Option.Value(), out var proposalThreshold))
            {
                proposalThreshold = DefaultValue;
            }

            Validated = true;
            return proposalThreshold / 100;
        }
    }
}