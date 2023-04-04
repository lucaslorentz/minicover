using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MiniCover.Core.Extensions
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

        public static IEnumerable<(SequencePoint sequencePoint, Instruction instruction)> MapSequencePointsToInstructions(
            this MethodDefinition methodDefinition)
        {
            var sequencePointsQueue = new Queue<SequencePoint>(methodDefinition.DebugInformation.SequencePoints);
            var instructionsQueue = new Queue<Instruction>(methodDefinition.Body.Instructions);

            if (sequencePointsQueue.Count > 0)
            {
                var sequencePoint = sequencePointsQueue.Dequeue();
                while (sequencePointsQueue.Count > 0)
                {
                    var nextSequencePoint = sequencePointsQueue.Dequeue();
                    while (instructionsQueue.Count > 0
                        && instructionsQueue.Peek().Offset < nextSequencePoint.Offset)
                    {
                        yield return (sequencePoint, instructionsQueue.Dequeue());
                    }

                    sequencePoint = nextSequencePoint;
                }

                while (instructionsQueue.Count > 0)
                {
                    var remainingInstruction = instructionsQueue.Dequeue();
                    yield return (sequencePoint, remainingInstruction);
                }
            }
        }

        public static MethodDefinition ResolveOriginalMethod(this MethodDefinition methodDefinition)
        {
            var originalMethodName = ExtractOriginalMethodName(methodDefinition.Name);

            if (originalMethodName == null && methodDefinition.DeclaringType != null)
                originalMethodName = ExtractOriginalMethodName(methodDefinition.DeclaringType.Name);

            if (!string.IsNullOrEmpty(originalMethodName)
                && methodDefinition.DeclaringType != null
                && methodDefinition.DeclaringType.IsCompilerGenerated()
                && methodDefinition.DeclaringType.DeclaringType != null)
            {
                var originalMethod = methodDefinition.DeclaringType.DeclaringType.Methods
                    .FirstOrDefault(m => m.Name == originalMethodName);

                if (originalMethod != null)
                    return originalMethod;
            }

            return methodDefinition;
        }

        private static string ExtractOriginalMethodName(string name)
        {
            var lessThanIndex = name.IndexOf("<");
            var greaterThanIndex = name.IndexOf(">");

            if (lessThanIndex == -1 || greaterThanIndex == -1 || lessThanIndex + 1 == greaterThanIndex)
                return null;

            return name.Substring(lessThanIndex + 1, greaterThanIndex - lessThanIndex - 1);
        }
    }
}
