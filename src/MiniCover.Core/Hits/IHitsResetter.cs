using System.IO.Abstractions;

namespace MiniCover.Core.Hits
{
    public interface IHitsResetter
    {
        bool ResetHits(IDirectoryInfo hitsDirectory);
    }
}