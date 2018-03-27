using System;
using System.IO;

namespace MiniCover.Commands.Options
{
    internal abstract class MiniCoverTouchOption : MiniCoverOption<string>
    {
        private readonly string _defaultValue;

        protected MiniCoverTouchOption(string defaultValue, string description, string optionTemplate)
                    : base(description, optionTemplate)
        {
            _defaultValue = defaultValue;
        }

        protected override string GetOptionValue()
        {
            return Option.Value() ?? _defaultValue;
        }

        protected override bool Validation()
        {
            TouchFile(ValueField);
            return true;
        }

        private void TouchFile(string path)
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
        }
    }
}