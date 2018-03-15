using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniCover.Commands.Options
{
    public abstract class IncludeFilesPatternOption : IMiniCoverOption<IEnumerable<string>>
    {
        protected abstract string DefaultValue { get; }

        private bool _invalidated;
        private CommandOption _option;
        private IEnumerable<string> _value;
        public abstract string Description { get; }
        public abstract string OptionTemplate { get; }
        public CommandOptionType Type => CommandOptionType.MultipleValue;

        public IEnumerable<string> Value
        {
            get
            {
                if (_invalidated) return _value;
                throw new MemberAccessException("Option should be invalidate before Value access");
            }
            set => _value = value;
        }

        public void Initialize(CommandLineApplication commandContext)
        {
            _option = commandContext.Option(OptionTemplate, Description, Type);
        }

        public void Invalidate()
        {
            var proposalValue = _option.Values ?? new List<string>();

            if (!string.IsNullOrWhiteSpace(DefaultValue))
                proposalValue.Add(DefaultValue);

            _value = proposalValue.Distinct();
            _invalidated = true;
        }
    }

    public class IncludeAssembliesPatternOption : IncludeFilesPatternOption
    {
        protected override string DefaultValue => "**/*.dll";
        public override string Description => $"Pattern to include assemblies [base: {DefaultValue}]";
        public override string OptionTemplate => "--assemblies";
    }

    public class ExcludeAssembliesPatternOption : IncludeFilesPatternOption
    {
        protected override string DefaultValue => null;
        public override string Description => "Pattern to exclude assemblies";
        public override string OptionTemplate => "--exclude-assemblies";
    }

    public class IncludeSourcesPatternOption : IncludeFilesPatternOption
    {
        protected override string DefaultValue => "**/*.cs";
        public override string Description => $"Pattern to include source files [base: {DefaultValue}]";
        public override string OptionTemplate => "--sources";
    }

    public class ExcludeSourcesPatternOption : IncludeFilesPatternOption
    {
        protected override string DefaultValue => null;
        public override string Description => "Pattern to exclude source files";
        public override string OptionTemplate => "--exclude-sources";
    }
}