using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using MiniCover.HitServices;

namespace MiniCover.Core.Hits
{
    public class HitsReader : IHitsReader
    {
        private readonly IFileSystem _fileSystem;

        public HitsReader(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public HitsInfo TryReadFromDirectory(string path)
        {
            var contexts = new List<HitContext>();

            if (_fileSystem.Directory.Exists(path))
            {
                foreach (var hitFile in _fileSystem.Directory.GetFiles(path, "*.hits"))
                {
                    using (var fileStream = _fileSystem.File.Open(hitFile, FileMode.Open, FileAccess.Read))
                    {
                        contexts.AddRange(HitContext.Deserialize(fileStream));
                    }
                }
            }

            return new HitsInfo(contexts);
        }
    }
}
