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

        public bool InstrumentType(
            IInstrumentationContext context,
            TypeDefinition typeDefinition,
            InstrumentedAssembly instrumentedAssembly)
        {
            var instrumented = false;
            
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

                instrumented = true;
            }

            foreach (var nestedType in typeDefinition.NestedTypes) {
                if (InstrumentType(context, nestedType, instrumentedAssembly))
                    instrumented = true;
            }

            return instrumented;
        }
    }
}
