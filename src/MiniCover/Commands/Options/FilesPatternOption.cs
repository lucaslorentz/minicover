using Microsoft.Extensions.CommandLineUtils;
using System.Collections.Generic;
using System.Linq;

namespace MiniCover.Commands.Options
{
    internal abstract class FilesPatternOption : MiniCoverOption<IEnumerable<string>>
    {
        private readonly string _defaultValue;

        protected FilesPatternOption(string defaultValue, string description, string optionTemplate)
            : base(description, optionTemplate, CommandOptionType.MultipleValue)
        {
            _defaultValue = defaultValue;
        }

        protected override IEnumerable<string> GetOptionValue()
        {
            var proposalValue = Option.Values ?? new List<string>();

            if (!string.IsNullOrWhiteSpace(_defaultValue))
                proposalValue.Add(_defaultValue);

            return proposalValue.Distinct();
        }

        protected override bool Validation() => true;
    }
}