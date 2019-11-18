using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MiniCover.Extensions
{
    public static class MethodDefinitionExtensions
    {
        public static IList<Document> GetAllDocuments(this MethodDefinition methodDefinition)
        {
            return methodDefinition.DebugInformation.SequencePoints
                .Select(c => c.Document)
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
