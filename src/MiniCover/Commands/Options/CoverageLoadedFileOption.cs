using MiniCover.Model;
using Newtonsoft.Json;
using System;
using System.IO;

namespace MiniCover.Commands.Options
{
    public class CoverageLoadedFileOption : CoverageFileOption, IMiniCoverOption<InstrumentationResult>
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

        public override void Invalidate()
        {
            base.Invalidate();

            if (!File.Exists(CoverageFilePath))
            {
                throw new ArgumentException($"Coverage file does not exist '{CoverageFilePath}'");
            }

            var coverageFileString = File.ReadAllText(CoverageFilePath);
            _value = JsonConvert.DeserializeObject<InstrumentationResult>(coverageFileString);
            _invalidated = true;
        }
    }
}