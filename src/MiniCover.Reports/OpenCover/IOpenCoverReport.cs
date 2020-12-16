using System.IO.Abstractions;
using MiniCover.Core.Model;

namespace MiniCover.Reports.OpenCover
{
    public interface IOpenCoverReport
    {
        void Execute(InstrumentationResult result, IFileInfo output);
    }
}