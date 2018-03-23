namespace MiniCover.Commands.Options
{
    internal class NCoverOutputOption : MiniCoverTouchOption
    {
        protected override string DefaultValue => "./coverage.xml";
        protected override string Description => $"Output file for NCover report [default: {DefaultValue}]";
        protected override string OptionTemplate => "--output";
    }
}