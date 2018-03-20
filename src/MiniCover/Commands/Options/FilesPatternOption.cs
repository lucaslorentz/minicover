using Microsoft.Extensions.CommandLineUtils;
using System.Collections.Generic;
using System.Linq;

namespace MiniCover.Commands.Options
{
    public abstract class FilesPatternOption : MiniCoverOption<IEnumerable<string>>
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

    internal class ExcludeAssembliesPatternOption : FilesPatternOption
    {
        protected override string DefaultValue => null;
        protected override string Description => "Pattern to exclude assemblies";
        protected override string OptionTemplate => "--exclude-assemblies";
    }

    internal class ExcludeSourcesPatternOption : FilesPatternOption
    {
        protected override string DefaultValue => null;
        protected override string Description => "Pattern to exclude source files";
        protected override string OptionTemplate => "--exclude-sources";
    }

    internal class IncludeAssembliesPatternOption : FilesPatternOption
    {
        protected override string DefaultValue => "**/*.dll";
        protected override string Description => $"Pattern to include assemblies [default: {DefaultValue}]";
        protected override string OptionTemplate => "--assemblies";
    }

    internal class IncludeSourcesPatternOption : FilesPatternOption
    {
        protected override string DefaultValue => "**/*.cs";
        protected override string Description => $"Pattern to include source files [default: {DefaultValue}]";
        protected override string OptionTemplate => "--sources";
    }
}