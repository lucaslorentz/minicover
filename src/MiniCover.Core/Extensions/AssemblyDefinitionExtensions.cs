using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MiniCover.Core.Extensions
{
    public static class AssemblyDefinitionExtensions
    {
        public static IList<Document> GetAllDocuments(this AssemblyDefinition assemblyDefinition)
        {
            return assemblyDefinition
                .MainModule.GetTypes()
                .SelectMany(t => t.GetAllDocuments(false))
                .Distinct()
                .ToArray();
        }

        public static bool IsExcludedFromCodeCoverage(this AssemblyDefinition assemblyDefinition)
        {
            return assemblyDefinition.HasExcludeFromCodeCoverageAttribute();
        }
    }
}
