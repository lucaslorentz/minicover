using MiniCover.Model;

namespace MiniCover.Reports.Helpers
{
    public interface ISummaryFactory
    {
        Summary CalculateSummary(InstrumentationResult result, float threshold);
    }
}