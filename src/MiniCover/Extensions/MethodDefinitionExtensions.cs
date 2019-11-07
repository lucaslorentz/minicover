using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace MiniCover.Extensions
{
    public static class MethodDefinitionExtensions
    {
        public static IList<string> GetAllDocuments(this MethodDefinition methodDefinition)
        {
            return methodDefinition.DebugInformation.SequencePoints
                .Select(c => c.Document.Url)
                .Distinct()
                .ToArray();
        }

        public static bool IsExcludedFromCodeCoverage(this MethodDefinition methodDefinition)
        {
            return methodDefinition.HasExcludeFromCodeCoverageAttribute()
                || methodDefinition.DeclaringType.IsExcludedFromCodeCoverage();
        }
    }
}
