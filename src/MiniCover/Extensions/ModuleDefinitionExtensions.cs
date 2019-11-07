using System;
using System.Collections.Concurrent;
using System.Reflection;
using Mono.Cecil;

namespace MiniCover.Extensions
{
    public static class ModuleDefinitionExtensions
    {
        public static TypeReference GetOrImportReference(this ModuleDefinition moduleDefinition, Type type)
        {
            var references = moduleDefinition.GetOrAddExtension("TypeReferences", () => new ConcurrentDictionary<Type, TypeReference>());
            return references.GetOrAdd(type, (t) => moduleDefinition.ImportReference(type));
        }

        public static MethodReference GetOrImportReference(this ModuleDefinition moduleDefinition, MethodInfo method)
        {
            var references = moduleDefinition.GetOrAddExtension("MethodReferences", () => new ConcurrentDictionary<MethodInfo, MethodReference>());
            return references.GetOrAdd(method, (t) => moduleDefinition.ImportReference(method));
        }

        public static bool IsExcludedFromCodeCoverage(this ModuleDefinition moduleDefinition)
        {
            return moduleDefinition.HasExcludeFromCodeCoverageAttribute()
                || moduleDefinition.Assembly.IsExcludedFromCodeCoverage();
        }
    }
}
