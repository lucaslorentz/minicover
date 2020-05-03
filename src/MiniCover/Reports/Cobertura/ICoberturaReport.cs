using System.IO.Abstractions;
using MiniCover.Model;

namespace MiniCover.Reports.Cobertura
{
    public interface ICoberturaReport
    {
        void Execute(InstrumentationResult result, IFileInfo output);
    }
}