using System;
using System.IO;
using YamlDotNet.Serialization;

namespace MiniCover.Commands.Options.FileParameterizations
{
    internal class ParameterizationOption : PathOption, IMiniCoverOption<FileParameterization>
    {
        private readonly IMiniCoverOption[] _options;
        private const string Description = "Opisowka";
        private const string OptionTemplate = "--file";
        private FileParameterization _value;

        internal ParameterizationOption(params IMiniCoverOption[] options)
            : base(null, Description, OptionTemplate)
        {
            _options = options;
        }
        
        FileParameterization IMiniCoverOption<FileParameterization>.GetValue()
        {
            if (Validated) return _value;
            throw new MemberAccessException($"File with parameterization does not exist '{ValueField}'");
        }

        protected override string GetOptionValue()
        {
            var result = base.GetOptionValue();
            var fileString = File.ReadAllText(result);

            var deserializer = new DeserializerBuilder().Build();
            _value = deserializer.Deserialize<FileParameterization>(fileString);

            return result;
        }

        protected override bool Validation()
        {
            return File.Exists(ValueField);
        }
    }
}