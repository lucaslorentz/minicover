using System.Collections.Generic;
using MiniCover.Core.Hits;
using MiniCover.Core.Model;

namespace MiniCover.Reports.Helpers
{
    public interface ISummaryFactory
    {
        Summary CalculateSummary(InstrumentationResult result, float threshold);

        Summary CalculateFilesSummary(IEnumerable<SourceFile> sourceFiles, HitsInfo hitsInfo, float threshold);

        List<SummaryRow> GetSummaryGrid(SourceFile[] sourceFiles, HitsInfo hitsInfo, float threshold);
    }
}