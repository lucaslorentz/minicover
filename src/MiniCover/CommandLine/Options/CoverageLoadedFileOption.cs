using System.IO;
using MiniCover.Exceptions;
using MiniCover.Model;
using Newtonsoft.Json;

namespace MiniCover.CommandLine.Options
{
    class CoverageLoadedFileOption : CoverageFileOption
    {
        public InstrumentationResult Result { get; private set; }

        protected override FileInfo PrepareValue(string value)
        {
            var fileInfo = base.PrepareValue(value);

            if (!fileInfo.Exists)
                throw new ValidationException($"Coverage file does not exist '{fileInfo.FullName}'");

            var coverageFileString = File.ReadAllText(fileInfo.FullName);
            Result = JsonConvert.DeserializeObject<InstrumentationResult>(coverageFileString);

            return fileInfo;
        }
    }
}