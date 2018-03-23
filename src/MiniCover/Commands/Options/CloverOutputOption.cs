namespace MiniCover.Commands.Options
{
    internal class CloverOutputOption : MiniCoverTouchOption
    {
        protected override string DefaultValue => "./clover.xml";
        protected override string Description => $"Output file for Clover report [default: {DefaultValue}]";
        protected override string OptionTemplate => "--output";
    }
}