using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace MiniCover.Model
{
    public class GraphNode
    {
        public Instruction Instruction { get; }

        public HashSet<GraphNode> Children { get; } = new HashSet<GraphNode>();

        public GraphNode(Instruction instruction)
        {
            Instruction = instruction;
        }
    }
}
