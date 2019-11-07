using System.IO;

namespace MiniCover.CommandLine.Options
{
    abstract class DirectoryOption : SingleValueOption<DirectoryInfo>
    {
        protected DirectoryOption(string template, string description)
            : base(template, description)
        {
        }

        protected override DirectoryInfo PrepareValue(string value)
        {
            return new DirectoryInfo(value ?? GetDefaultValue());
        }

        protected abstract string GetDefaultValue();
    }
}