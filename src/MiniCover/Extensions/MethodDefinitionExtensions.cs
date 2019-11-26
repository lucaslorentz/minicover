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

        public static IEnumerable<(SequencePoint sequencePoint, Instruction instruction)> MapSequencePointsToInstructions(
            this MethodDefinition methodDefinition)
        {
            var sequencePointsQueue = new Queue<SequencePoint>(methodDefinition.DebugInformation.SequencePoints);
            var instructionsQueue = new Queue<Instruction>(methodDefinition.Body.Instructions);

            if (sequencePointsQueue.TryDequeue(out var sequencePoint))
            {
                while (sequencePointsQueue.TryDequeue(out var nextSequencePoint))
                {
                    while (instructionsQueue.TryPeek(out var instruction)
                        && instruction.Offset < nextSequencePoint.Offset)
                    {
                        yield return (sequencePoint, instructionsQueue.Dequeue());
                    }

                    sequencePoint = nextSequencePoint;
                }

                while (instructionsQueue.TryDequeue(out var remainingInstruction))
                {
                    yield return (sequencePoint, remainingInstruction);
                }
            }
        }

        public static MethodDefinition ResolveOriginalMethod(this MethodDefinition methodDefinition)
        {
            var originalMethodName = ExtractOriginalMethodName(methodDefinition.Name)
                ?? ExtractOriginalMethodName(methodDefinition.DeclaringType.Name);

            if (!string.IsNullOrEmpty(originalMethodName)
                && methodDefinition.DeclaringType.IsCompilerGenerated())
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
