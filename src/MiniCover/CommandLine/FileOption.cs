using System.IO;

namespace MiniCover.CommandLine.Options
{
    abstract class FileOption : SingleValueOption<FileInfo>
    {
        protected FileOption(string template, string description)
            : base(template, description)
        {
        }

        protected override FileInfo PrepareValue(string value)
        {
            return new FileInfo(value ?? GetDefaultValue());
        }

        protected abstract string GetDefaultValue();
    }
}