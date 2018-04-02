using System;

namespace MiniCover.Commands.Options
{
    public class VerbosityOption : MiniCoverOption<Verbosity>
    {
        private const Verbosity DefaultValue = Verbosity.Normal;
        private const string OptionTemplate = "--verbosity";

        private static readonly string Description = $"Verbosity [default: {DefaultValue}]";

        public VerbosityOption() : base(Description, OptionTemplate)
        {
        }

        protected override Verbosity GetOptionValue()
        {
            if (!Enum.TryParse(Option.Value(), out Verbosity proposalVerbosity))
            {
                proposalVerbosity = DefaultValue;
            }

            return proposalVerbosity;
        }

        protected override bool Validation() => true;
    }

    public enum Verbosity
    {
        Quiet,
        Normal
    }
}