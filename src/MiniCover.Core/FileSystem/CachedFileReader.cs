using System.IO;
using System.IO.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace MiniCover.Core.FileSystem
{
    public class CachedFileReader : IFileReader
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IFileSystem _fileSystem;

        public CachedFileReader(
            IMemoryCache memoryCache,
            IFileSystem fileSystem)
        {
            _memoryCache = memoryCache;
            _fileSystem = fileSystem;
        }

        public string[] ReadAllLines(FileInfo file)
        {
            return _memoryCache.GetOrCreate<string[]>(file.FullName, entry =>
            {
                entry.Priority = CacheItemPriority.Low;
                return _fileSystem.File.ReadAllLines(file.FullName);
            });
        }
    }
}
