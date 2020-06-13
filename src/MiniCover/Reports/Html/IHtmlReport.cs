using System.IO.Abstractions;
using MiniCover.Model;

namespace MiniCover.Reports.Html
{
    public interface IHtmlReport
    {
        int Execute(InstrumentationResult result, IDirectoryInfo output, float threshold, bool noFail);
    }
}