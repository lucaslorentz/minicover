using Microsoft.Extensions.CommandLineUtils;
using System.Collections.Generic;
using System.Linq;

namespace MiniCover.Commands.Options
{
    internal abstract class FilesPatternOption : MiniCoverOption<IEnumerable<string>>
    {
        protected abstract string DefaultValue { get; }

        protected override CommandOptionType Type => CommandOptionType.MultipleValue;

        protected override IEnumerable<string> GetOptionValue()
        {
            var proposalValue = Option.Values ?? new List<string>();

            if (!string.IsNullOrWhiteSpace(DefaultValue))
                proposalValue.Add(DefaultValue);

            return proposalValue.Distinct();
        }
    }
}