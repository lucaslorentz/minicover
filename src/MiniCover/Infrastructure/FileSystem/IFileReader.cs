using System.IO;

namespace MiniCover.Infrastructure.FileSystem
{
    public interface IFileReader
    {
        string[] ReadAllLines(FileInfo file);
    }
}