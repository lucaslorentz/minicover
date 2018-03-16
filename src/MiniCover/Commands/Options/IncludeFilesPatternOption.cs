using Microsoft.Extensions.CommandLineUtils;
using System.Collections.Generic;
using System.Linq;

namespace MiniCover.Commands.Options
{
    internal abstract class IncludeFilesPatternOption : MiniCoverOption<IEnumerable<string>>
    {
        protected abstract string DefaultValue { get; }

        public override CommandOptionType Type => CommandOptionType.MultipleValue;

        public override void Invalidate()
        {
            var proposalValue = Option.Values ?? new List<string>();

            if (!string.IsNullOrWhiteSpace(DefaultValue))
                proposalValue.Add(DefaultValue);

            ValueField = proposalValue.Distinct();
            Invalidated = true;
        }
    }

    internal class IncludeAssembliesPatternOption : IncludeFilesPatternOption
    {
        protected override string DefaultValue => "**/*.dll";
        public override string Description => $"Pattern to include assemblies [base: {DefaultValue}]";
        public override string OptionTemplate => "--assemblies";
    }

    internal class ExcludeAssembliesPatternOption : IncludeFilesPatternOption
    {
        protected override string DefaultValue => null;
        public override string Description => "Pattern to exclude assemblies";
        public override string OptionTemplate => "--exclude-assemblies";
    }

    internal class IncludeSourcesPatternOption : IncludeFilesPatternOption
    {
        protected override string DefaultValue => "**/*.cs";
        public override string Description => $"Pattern to include source files [base: {DefaultValue}]";
        public override string OptionTemplate => "--sources";
    }

    internal class ExcludeSourcesPatternOption : IncludeFilesPatternOption
    {
        protected override string DefaultValue => null;
        public override string Description => "Pattern to exclude source files";
        public override string OptionTemplate => "--exclude-sources";
    }
}