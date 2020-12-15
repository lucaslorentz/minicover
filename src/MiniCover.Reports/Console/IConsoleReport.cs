using MiniCover.Core.Model;

namespace MiniCover.Reports.Console
{
    public interface IConsoleReport
    {
        int Execute(InstrumentationResult result, float threshold, bool noFail);
    }
}