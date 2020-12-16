using System.IO;

namespace MiniCover.Core.FileSystem
{
    public interface IFileReader
    {
        string[] ReadAllLines(FileInfo file);
    }
}