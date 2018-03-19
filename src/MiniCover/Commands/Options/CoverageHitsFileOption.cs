using Microsoft.Extensions.CommandLineUtils;

namespace MiniCover.Commands.Options
{
    internal class CoverageHitsFileOption : MiniCoverOption<string>
    {
        private const string defaultValue = "coverage-hits.txt";
        protected override string Description => $"Hits file name pattern [default: {defaultValue}]";
        protected override string OptionTemplate => "--hits-file";

        protected override string GetOptionValue(CommandOption option)
        {
            var optionValue = option.Value();

            if (string.IsNullOrWhiteSpace(optionValue))
                return defaultValue;

            return optionValue;
        }
    }
}