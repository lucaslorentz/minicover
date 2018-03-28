using MiniCover.Commands.Options.FileParameterizations;
using System;

namespace MiniCover.Commands.Options
{
    internal class ThresholdOption : MiniCoverOption<float>, IMiniCoverParameterizationOption
    {
        private const float DefaultValue = 90;
        private const string OptionTemplate = "--threshold";

        private static readonly string Description = $"Coverage percentage threshold [default: {DefaultValue}]";

        public ThresholdOption() : base(Description, OptionTemplate)
        {
        }

        public Action<MiniCoverParameterization> SetParameter =>
            parameterization =>
                parameterization.Threshold = GetValue();

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