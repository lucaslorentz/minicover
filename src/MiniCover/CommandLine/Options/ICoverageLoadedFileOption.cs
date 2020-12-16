using MiniCover.Core.Model;

namespace MiniCover.CommandLine.Options
{
    public interface ICoverageLoadedFileOption : IOption
    {
        InstrumentationResult Result { get; }
    }
}