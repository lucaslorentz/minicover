using System.Collections.Generic;
using MiniCover.Core.Model;
using Mono.Cecil.Cil;

namespace MiniCover.Core.Extensions
{
    public static class InstructionExtensions
    {
        public static GraphNode<Instruction> ToGraph(this Instruction rootInstruction)
        {
            var cache = new Dictionary<Instruction, GraphNode<Instruction>>();

            var rootNode = new GraphNode<Instruction>(rootInstruction);
            cache[rootInstruction] = rootNode;

            var pending = new Queue<GraphNode<Instruction>>();
            pending.Enqueue(rootNode);

            while (pending.Count > 0)
            {
                var node = pending.Dequeue();
                foreach (var childInstruction in node.Value.GetChildren())
                {
                    if (!cache.TryGetValue(childInstruction, out var childNode))
                    {
                        childNode = new GraphNode<Instruction>(childInstruction);
                        cache[childInstruction] = childNode;
                        pending.Enqueue(childNode);
                    }

                    node.Children.Add(childNode);
                }
            }

            return rootNode;
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
