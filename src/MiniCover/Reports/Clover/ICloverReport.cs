using System.IO.Abstractions;
using MiniCover.Model;

namespace MiniCover.Reports.Clover
{
    public interface ICloverReport
    {
        void Execute(InstrumentationResult result, IFileInfo output);
    }
}