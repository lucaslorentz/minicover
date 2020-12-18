using MiniCover.Core.Model;
using Mono.Cecil;

namespace MiniCover.Core.Instrumentation
{
    public interface ITypeInstrumenter
    {
        bool InstrumentType(IInstrumentationContext context, TypeDefinition typeDefinition, InstrumentedAssembly instrumentedAssembly);
    }
}