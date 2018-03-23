namespace MiniCover.Commands.Options
{
    internal class OpenCoverOutputOption : MiniCoverTouchOption
    {
        protected override string DefaultValue => "./opencovercoverage.xml";
        protected override string Description => $"Output file for OpenCover report [default: {DefaultValue}]";
        protected override string OptionTemplate => "--output";
    }
}