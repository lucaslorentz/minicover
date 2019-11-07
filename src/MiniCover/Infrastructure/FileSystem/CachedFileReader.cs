using System.IO;
using Microsoft.Extensions.Caching.Memory;

namespace MiniCover.Infrastructure.FileSystem
{
    public class CachedFileReader : IFileReader
    {
        private readonly IMemoryCache _memoryCache;

        public CachedFileReader(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public string[] ReadAllLines(FileInfo file)
        {
            return _memoryCache.GetOrCreate<string[]>(file.FullName, entry =>
            {
                entry.Priority = CacheItemPriority.Low;
                return File.ReadAllLines(file.FullName);
            });
        }
    }
}
