using System.IO.Abstractions;
using MiniCover.Core.Model;

namespace MiniCover.Core.Instrumentation
{
    public interface IAssemblyInstrumenter
    {
        InstrumentedAssembly InstrumentAssemblyFile(IInstrumentationContext context, IFileInfo assemblyFile);
    }
}