using System.IO.Abstractions;
using MiniCover.Exceptions;
using MiniCover.Model;
using Newtonsoft.Json;

namespace MiniCover.CommandLine.Options
{
    public class CoverageLoadedFileOption : CoverageFileOption
    {
        private readonly IFileSystem _fileSystem;

        public CoverageLoadedFileOption(IFileSystem fileSystem)
            : base(fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public InstrumentationResult Result { get; private set; }

        public override void ReceiveValue(string value)
        {
            base.ReceiveValue(value);

            if (!FileInfo.Exists)
                throw new ValidationException($"Coverage file does not exist '{FileInfo.FullName}'");

            var coverageFileString = _fileSystem.File.ReadAllText(FileInfo.FullName);
            Result = JsonConvert.DeserializeObject<InstrumentationResult>(coverageFileString);
        }
    }
}