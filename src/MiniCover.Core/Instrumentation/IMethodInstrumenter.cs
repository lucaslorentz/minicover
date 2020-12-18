using MiniCover.Core.Model;
using Mono.Cecil;

namespace MiniCover.Core.Instrumentation
{
    public interface IMethodInstrumenter
    {
        void InstrumentMethod(IInstrumentationContext context, bool instrumentInstructions, MethodDefinition methodDefinition, InstrumentedAssembly instrumentedAssembly);
    }
}