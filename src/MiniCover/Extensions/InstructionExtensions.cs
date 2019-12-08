using System.Collections.Generic;
using MiniCover.Model;
using Mono.Cecil.Cil;

namespace MiniCover.Extensions
{
    public static class InstructionExtensions
    {
        public static GraphNode ToGraph(this Instruction rootInstruction)
        {
            var cache = new Dictionary<Instruction, GraphNode>();

            GraphNode Visit(Instruction instruction)
            {
                if (cache.ContainsKey(instruction))
                    return cache[instruction];

                var node = new GraphNode(instruction);
                cache[instruction] = node;
                foreach (var child in instruction.GetChildren())
                    node.Children.Add(Visit(child));

                return node;
            }

            return Visit(rootInstruction);
        }

        public static IEnumerable<Instruction> GetChildren(this Instruction instruction)
        {
            switch (instruction.OpCode.FlowControl)
            {
                case FlowControl.Cond_Branch:
                    yield return instruction.Next;

                    if (instruction.Operand is Instruction operand)
                        yield return operand;
                    else if (instruction.Operand is Instruction[] operands)
                        foreach (var i in operands)
                            yield return i;

                    break;
                case FlowControl.Branch:
                    yield return instruction.Operand as Instruction;
                    break;
                default:
                    if (instruction.Next != null)
                        yield return instruction.Next;

                    break;
            }
        }
    }
}
