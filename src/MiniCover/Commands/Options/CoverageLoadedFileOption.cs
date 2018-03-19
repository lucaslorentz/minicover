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

        InstrumentationResult IMiniCoverOption<InstrumentationResult>.Value
        {
            get
            {
                if (_invalidated) return _value;
                throw new MemberAccessException("Option should be invalidate before Value access");
            }
        }

        public override void Validate()
        {
            base.Validate();

            if (!File.Exists(Value))
            {
                throw new ArgumentException($"Coverage file does not exist '{Value}'");
            }

            var coverageFileString = File.ReadAllText(Value);
            _value = JsonConvert.DeserializeObject<InstrumentationResult>(coverageFileString);
            _invalidated = true;
        }
    }
}