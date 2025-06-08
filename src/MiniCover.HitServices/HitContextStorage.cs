using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace MiniCover.HitServices
{
    public static class HitContextStorage
    {
        private static readonly ConcurrentDictionary<string, MemoryStream> _storage = new ConcurrentDictionary<string, MemoryStream>();
    
        public static void Save(HitContext hitContext, string hitsPath)
        {
            var fileName = Path.Combine(hitsPath, $"{hitContext.Id}.hits");

            var stream = new MemoryStream();
            hitContext.Serialize(stream);
            stream.Flush();
            stream.Dispose();
            _storage.TryAdd(fileName, stream);
        }

        public static void Flush()
        {
            foreach (var kvp in _storage)
            {
                var fileName = kvp.Key;
                var path = Path.GetDirectoryName(fileName) ?? throw new InvalidOperationException($"Cannot get directory name for {fileName}.");
                Directory.CreateDirectory(path);

                var memoryStream = kvp.Value;
                
                using (var fileStream = File.Open(fileName, FileMode.Create))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.CopyTo(fileStream);
                    fileStream.Flush();
                }
            }

            _storage.Clear();
        }
        
        public static bool Clear(string hitsPath)
        {
            _storage.Clear();
            foreach (var kvp in _storage)
            {
                kvp.Value.Dispose();
            }
            var hitsDirectory = new DirectoryInfo(hitsPath);

            var hitsFiles = hitsDirectory.Exists
                ? hitsDirectory.GetFiles("*.hits")
                : Array.Empty<FileInfo>();

            if (!hitsFiles.Any())
            {
                return true;
            }

            var errorsCount = 0;
            foreach (var hitsFile in hitsFiles)
            {
                try
                {
                    hitsFile.Delete();
                }
                catch (Exception e)
                {
                    errorsCount++;
                }
            }

            if (errorsCount != 0)
            {
                return false;
            }

            return true;
        }
    }
}
