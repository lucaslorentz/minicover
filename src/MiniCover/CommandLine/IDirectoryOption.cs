using System.IO.Abstractions;

namespace MiniCover.CommandLine.Options
{
    public interface IDirectoryOption : IOption
    {
        IDirectoryInfo DirectoryInfo { get; }
    }
}