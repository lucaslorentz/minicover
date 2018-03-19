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
            Validated = true;
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

    internal class NCoverOutputOption : MiniCoverTouchOption
    {
        protected override string DefaultValue => "./coverage.xml";
        protected override string Description => $"Output file for NCover report [default: {DefaultValue}]";
        protected override string OptionTemplate => "--output";
    }

    internal class CloverOutputOption : MiniCoverTouchOption
    {
        protected override string DefaultValue => "./clover.xml";
        protected override string Description => $"Output file for Clover report [default: {DefaultValue}]";
        protected override string OptionTemplate => "--output";
    }

    internal class OpenCoverOutputOption : MiniCoverTouchOption
    {
        protected override string DefaultValue => "./opencovercoverage.xml";
        protected override string Description => $"Output file for OpenCover report [default: {DefaultValue}]";
        protected override string OptionTemplate => "--output";
    }
}
