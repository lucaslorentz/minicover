namespace MiniCover.Commands.Options.Reports
{
    internal class NCoverOutputOption : MiniCoverTouchOption
    {
        private const string DefaultValue = "./coverage.xml";
        private const string OptionTemplate = "--output";

        private static readonly string Description = $"Output file for NCover report [default: {DefaultValue}]";

        internal NCoverOutputOption()
            : base(DefaultValue, Description, OptionTemplate)
        {
        }
    }
}