namespace MiniCover.Commands.Options.Reports
{
    internal class OpenCoverOutputOption : MiniCoverTouchOption
    {
        private const string DefaultValue = "./opencovercoverage.xml";
        private const string OptionTemplate = "--output";

        private static readonly string Description = $"Output file for OpenCover report [default: {DefaultValue}]";

        internal OpenCoverOutputOption()
            : base(DefaultValue, Description, OptionTemplate)
        {
        }
    }
}