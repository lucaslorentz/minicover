using Microsoft.Extensions.CommandLineUtils;
using System.IO;

namespace MiniCover.Commands.Options
{
    internal class WorkingDirOption : MiniCoverOption<DirectoryInfo>
    {
        private const string defaultValue = "./";
        protected override string Description => $"Solution directory [default: {defaultValue}]";
        protected override string OptionTemplate => "--workdir";

        protected override DirectoryInfo GetOptionValue(CommandOption option)
        {
            var directory = new DirectoryInfo(option.Value() ?? defaultValue);
            if (!directory.Exists)
                directory.Create();
            return directory;
        }
    }
}