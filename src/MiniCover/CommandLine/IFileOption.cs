using System.IO.Abstractions;

namespace MiniCover.CommandLine.Options
{
    public interface IFileOption : IOption
    {
        IFileInfo FileInfo { get; }
    }
}