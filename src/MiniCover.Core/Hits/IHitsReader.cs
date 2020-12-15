namespace MiniCover.Core.Hits
{
    public interface IHitsReader
    {
        HitsInfo TryReadFromDirectory(string path);
    }
}