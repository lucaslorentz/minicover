using MiniCover.Core.Model;
using Mono.Cecil;

namespace MiniCover.Core.Instrumentation
{
    public interface ITypeInstrumenter
    {
        void InstrumentType(IInstrumentationContext context, TypeDefinition typeDefinition, InstrumentedAssembly instrumentedAssembly);
    }
}