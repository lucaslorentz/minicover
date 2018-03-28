using MiniCover.Commands.Options.FileParameterizations;
using MiniCover.Model;
using Newtonsoft.Json;
using System;
using System.IO;

namespace MiniCover.Commands.Options
{
    internal class CoverageLoadedFileOption : CoverageFileOption, IMiniCoverOption<InstrumentationResult>, IMiniCoverParameterizationOption
    {
        private InstrumentationResult _value;

        public Action<MiniCoverParameterization> SetParameter =>
            parameterization => parameterization.InstrumentationResult = _value;

        InstrumentationResult IMiniCoverOption<InstrumentationResult>.GetValue()
        {
            if (Validated) return _value;
            throw new MemberAccessException($"Coverage file does not exist '{ValueField}'");
        }

        protected override string GetOptionValue()
        {
            var result = base.GetOptionValue();
            var coverageFileString = File.ReadAllText(result);
            _value = JsonConvert.DeserializeObject<InstrumentationResult>(coverageFileString);

            return result;
        }

        protected override bool Validation()
        {
            return File.Exists(ValueField);
        }
    }
}