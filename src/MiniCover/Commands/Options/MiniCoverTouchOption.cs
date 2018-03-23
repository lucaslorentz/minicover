using System;
using System.IO;

namespace MiniCover.Commands.Options
{
    internal abstract class MiniCoverTouchOption : MiniCoverOption<string>
    {
        protected abstract string DefaultValue { get; }

        protected override string GetOptionValue()
        {
            var coverageFilePath = Option.Value() ?? DefaultValue;
            return TouchFile(coverageFilePath);
        }

        private string TouchFile(string path)
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

            return new FileInfo(path).FullName;
        }
    }
}
