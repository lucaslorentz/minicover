using System;
using System.IO;
using YamlDotNet.Serialization;

namespace MiniCover.Commands.Options.FileParameterizations
{
    internal class ParameterizationOption : PathOption, IMiniCoverOption<MiniCoverParameterization>
    {
        private const string Description = "Parametrization file path";
        private const string OptionTemplate = "--parameterization-file";
        private readonly IMiniCoverParameterizationOption[] _options;
        private MiniCoverParameterization _value;

        internal ParameterizationOption(params IMiniCoverParameterizationOption[] options)
            : base(null, Description, OptionTemplate)
        {
            _options = options;
        }

        MiniCoverParameterization IMiniCoverOption<MiniCoverParameterization>.GetValue()
        {
            if (Validated) return _value;
            throw new MemberAccessException($"File with parameterization does not exist '{ValueField}'");
        }

        public override IMiniCoverOption[] NestedOptions() => _options;

        protected override string GetOptionValue()
        {
            var result = base.GetOptionValue();
            if (File.Exists(result))
            {
                var fileString = File.ReadAllText(result);
                var deserializer = new DeserializerBuilder().Build();
                _value = deserializer.Deserialize<MiniCoverParameterization>(fileString);
            }
            else
            {
                _value = new MiniCoverParameterization();
            }

            foreach (var miniCoverOption in _options)
            {
                miniCoverOption.SetParameter(_value);
            }
            return result;
        }
    }
}