using System.Collections.Generic;
using MiniCover.Core.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MiniCover.Core.Instrumentation.Patterns
{
    public static class LambdaInitPattern
    {
        public static IEnumerable<Instruction> FindInstructions(IList<Instruction> instructions)
        {
            for (var i = 0; i < instructions.Count; i++)
            {
                var openInstruction = instructions[i];

                if (openInstruction.OpCode.Code == Code.Ldsfld
                    && openInstruction.Operand is FieldDefinition fieldDefinitionI
                    && fieldDefinitionI.DeclaringType.IsCompilerGenerated())
                {
                    for (; i < instructions.Count; i++)
                    {
                        var currentInstruction = instructions[i];

                        yield return currentInstruction;

                        if (currentInstruction.OpCode.Code == Code.Stsfld
                            && currentInstruction.Operand is FieldDefinition fieldDefinitionJ
                            && fieldDefinitionJ == fieldDefinitionI)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
