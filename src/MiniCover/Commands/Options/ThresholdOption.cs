namespace MiniCover.Commands.Options
{
    internal class ThresholdOption : MiniCoverOption<float>
    {
        private const float DefaultValue = 90;
        private const string OptionTemplate = "--threshold";

        private static readonly string Description = $"Coverage percentage threshold [default: {DefaultValue}]";

        public ThresholdOption() : base(Description, OptionTemplate)
        {
        }

        protected override float GetOptionValue()
        {
            if (!float.TryParse(Option.Value(), out var proposalThreshold))
            {
                proposalThreshold = DefaultValue;
            }

            return proposalThreshold / 100;
        }

        protected override bool Validation() => true;
    }
}