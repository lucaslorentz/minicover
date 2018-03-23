using MiniCover.Model;
using Newtonsoft.Json;
using System;
using System.IO;

namespace MiniCover.Commands.Options
{
    internal class CoverageLoadedFileOption : CoverageFileOption, IMiniCoverOption<InstrumentationResult>
    {
        private bool _invalidated;
        private InstrumentationResult _value;

        InstrumentationResult IMiniCoverOption<InstrumentationResult>.GetValue()
        {
            if (_invalidated) return _value;
            throw new MemberAccessException("Option should be invalidate before GetValue access");
        }

        public override void Validate()
        {
            base.Validate();
            var value = GetValue();

            if (!File.Exists(value))
            {
                throw new ArgumentException($"Coverage file does not exist '{value}'");
            }

            var coverageFileString = File.ReadAllText(value);
            _value = JsonConvert.DeserializeObject<InstrumentationResult>(coverageFileString);
            _invalidated = true;
        }
    }
}