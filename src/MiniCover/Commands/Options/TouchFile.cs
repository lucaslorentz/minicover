using System;
using System.IO;

namespace MiniCover.Commands.Options
{
    public class TouchFile
    {
        public TouchFile(string path)
        {
            var directoryContext = Path.GetDirectoryName("./");

            Directory.CreateDirectory(directoryContext);
            if (!File.Exists(path))
            {
                using (File.Create(path)) { }
            }
            else
            {
                File.SetLastWriteTimeUtc(path, DateTime.UtcNow);
            }

            FullName = new FileInfo(path).FullName;
        }

        public string FullName { get; }
    }
}