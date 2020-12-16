using System.IO.Abstractions;
using MiniCover.Core.Model;

namespace MiniCover.Reports.Clover
{
    public interface ICloverReport
    {
        void Execute(InstrumentationResult result, IFileInfo output);
    }
}