namespace MiniCover.CommandLine.Options
{
    class ThresholdOption : SingleValueOption<float>
    {
        private const float _defaultValue = 90;
        private const string _template = "--threshold";
        private static readonly string _description = $"Coverage percentage threshold [default: {_defaultValue}]";

        public ThresholdOption()
            : base(_template, _description)
        {
        }

        protected override float PrepareValue(string value)
        {
            if (!float.TryParse(value, out var proposalThreshold))
            {
                proposalThreshold = _defaultValue;
            }

            return proposalThreshold / 100;
        }
    }
}