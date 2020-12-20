using MiniCover.Core.Model;
using Mono.Cecil;

namespace MiniCover.Core.Instrumentation
{
    public class TypeInstrumenter : ITypeInstrumenter
    {
        private readonly IMethodInstrumenter _methodInstrumenter;

        public TypeInstrumenter(IMethodInstrumenter methodInstrumenter)
        {
            _methodInstrumenter = methodInstrumenter;
        }

        public void InstrumentType(
            IInstrumentationContext context,
            TypeDefinition typeDefinition,
            InstrumentedAssembly instrumentedAssembly)
        {
            foreach (var methodDefinition in typeDefinition.Methods)
            {
                if (!methodDefinition.HasBody || !methodDefinition.DebugInformation.HasSequencePoints)
                    continue;

                var isTest = context.IsTest(methodDefinition);
                var isSource = context.IsSource(methodDefinition);
                if (!isSource && !isTest)
                    continue;

                _methodInstrumenter.InstrumentMethod(
                    context,
                    isSource,
                    methodDefinition,
                    instrumentedAssembly);
            }

            foreach (var nestedType in typeDefinition.NestedTypes)
                InstrumentType(context, nestedType, instrumentedAssembly);
        }
    }
}
