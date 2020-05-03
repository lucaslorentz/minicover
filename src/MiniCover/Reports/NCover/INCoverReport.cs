using System.IO.Abstractions;
using MiniCover.Model;

namespace MiniCover.Reports.NCover
{
    public interface INCoverReport
    {
        void Execute(InstrumentationResult result, IFileInfo output);
    }
}