using System.IO.Abstractions;
using MiniCover.Model;

namespace MiniCover.Reports.OpenCover
{
    public interface IOpenCoverReport
    {
        void Execute(InstrumentationResult result, IFileInfo output);
    }
}