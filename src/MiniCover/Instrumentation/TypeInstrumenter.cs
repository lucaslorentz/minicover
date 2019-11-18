using System.Linq;
using MiniCover.Extensions;
using MiniCover.Model;
using Mono.Cecil;

namespace MiniCover.Instrumentation
{
    public class TypeInstrumenter
    {
        private readonly MethodInstrumenter _methodInstrumenter;

        public TypeInstrumenter(MethodInstrumenter methodInstrumenter)
        {
            _methodInstrumenter = methodInstrumenter;
        }

        public void InstrumentType(
            InstrumentationContext context,
            TypeDefinition typeDefinition,
            InstrumentedAssembly instrumentedAssembly)
        {
            var typeDocumentsUrls = typeDefinition.GetAllDocuments()
                .Select(d => d.Url)
                .Distinct()
                .ToArray();

            if (!typeDocumentsUrls.Any(d => context.IsSource(d) || context.IsTest(d)))
            {
                return;
            }

            var methods = typeDefinition.GetAllMethods();

            foreach (var methodDefinition in methods)
            {
                var methodDocuments = methodDefinition.GetAllDocuments();

                var isSource = methodDocuments.Any(d => context.IsSource(d.Url));
                var isTest = methodDocuments.Any(d => context.IsTest(d.Url));

                if (!isSource && !isTest)
                    continue;

                _methodInstrumenter.InstrumentMethod(
                    context,
                    isSource,
                    methodDefinition,
                    instrumentedAssembly);
            }
        }
    }
}
