using MiniCover.Model;
using Newtonsoft.Json;
using System;
using System.IO;

namespace MiniCover.Commands.Options
{
    internal class CoverageLoadedFileOption : CoverageFileOption, IMiniCoverOption<InstrumentationResult>
    {
        private InstrumentationResult _value;

        InstrumentationResult IMiniCoverOption<InstrumentationResult>.GetValue()
        {
            if (Validated) return _value;
            throw new MemberAccessException($"Coverage file at the path '{ ValueField }' does not exist!");
        }

        protected override string GetOptionValue()
        {
            var result = base.GetOptionValue();
            if (!File.Exists(result))
            {
                throw new FileLoadException($"Coverage file at the path '{ result }' does not exist!");
            }
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