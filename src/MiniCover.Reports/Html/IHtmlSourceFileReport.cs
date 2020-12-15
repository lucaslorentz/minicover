using MiniCover.Core.Hits;
using MiniCover.Core.Model;

namespace MiniCover.Reports.Html
{
    public interface IHtmlSourceFileReport
    {
        void Generate(InstrumentationResult result, SourceFile sourceFile, HitsInfo hitsInfo, float threshold, string outputFile);
    }
}