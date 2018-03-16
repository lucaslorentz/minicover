namespace MiniCover.Commands.Options
{
    internal abstract class MiniCoverTouchOption : MiniCoverOption<string>
    {
        protected abstract string DefaultValue { get; }

        public override void Invalidate()
        {
            var coverageFilePath = Option.Value() ?? DefaultValue;
            ValueField = new TouchFile(coverageFilePath).FullName;
            Invalidated = true;
        }
    }

    internal class NCoverOutputOption : MiniCoverTouchOption
    {
        protected override string DefaultValue => "./coverage.xml";
        public override string Description => $"Output file for NCover report [default: {DefaultValue}]";
        public override string OptionTemplate => "--output";
    }

    internal class CloverOutputOption : MiniCoverTouchOption
    {
        protected override string DefaultValue => "./clover.xml";
        public override string Description => $"Output file for Clover report [default: {DefaultValue}]";
        public override string OptionTemplate => "--output";
    }

    internal class OpenCoverOutputOption : MiniCoverTouchOption
    {
        protected override string DefaultValue => "./opencovercoverage.xml";
        public override string Description => $"Output file for OpenCover report [default: {DefaultValue}]";
        public override string OptionTemplate => "--output";
    }
}