using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MiniCover.Core.Extensions
{
    public static class TypeDefinitionExtensions
    {
        public static IList<Document> GetAllDocuments(this TypeDefinition typeDefinition, bool includeNestedTypes)
        {
            return typeDefinition.GetAllMethods(includeNestedTypes)
                .SelectMany(m => m.GetAllDocuments())
                .Distinct()
                .ToArray();
        }

        public static IEnumerable<MethodDefinition> GetAllMethods(this TypeDefinition typeDefinition, bool includeNestedTypes)
        {
            foreach (var method in typeDefinition.Methods.Where(m => m.HasBody && m.DebugInformation.HasSequencePoints))
                yield return method;

            if (includeNestedTypes)
            {
                foreach (var subType in typeDefinition.NestedTypes)
                {
                    foreach (var method in GetAllMethods(subType, includeNestedTypes))
                        yield return method;
                }
            }
        }

        public static bool IsCompilerGenerated(this TypeDefinition typeDefinition)
        {
            return typeDefinition.HasCompilerGeneratedAttribute()
                || (typeDefinition.DeclaringType?.IsCompilerGenerated() ?? false);
        }

        public static bool IsExcludedFromCodeCoverage(this TypeDefinition typeDefinition)
        {
            if (typeDefinition.HasExcludeFromCodeCoverageAttribute())
                return true;

            if (typeDefinition.DeclaringType != null)
                return typeDefinition.DeclaringType.IsExcludedFromCodeCoverage();

            return typeDefinition.Module.IsExcludedFromCodeCoverage();
        }
    }
}
