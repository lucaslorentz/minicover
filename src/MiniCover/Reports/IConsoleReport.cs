using MiniCover.Model;

namespace MiniCover.Reports
{
    public interface IConsoleReport
    {
        int Execute(InstrumentationResult result, float threshold);
    }
}