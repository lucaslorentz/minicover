namespace MiniCover.Commands.Options
{
    internal class CloverOutputOption : MiniCoverTouchOption
    {
        private const string DefaultValue = "./clover.xml";
        private const string OptionTemplate = "--output";
        private static readonly string Description = $"Output file for Clover report [default: {DefaultValue}]";

        public CloverOutputOption()
            : base(DefaultValue, Description, OptionTemplate)
        {
        }
    }
}