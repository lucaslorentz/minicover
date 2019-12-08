using System.Collections.Generic;
using MiniCover.Model;
using Mono.Cecil.Cil;

namespace MiniCover.Extensions
{
    public static class GraphNodeExtensions
    {
        public static HashSet<GraphNode> Filter(
            this GraphNode rootNode,
            HashSet<Instruction> allowedInstructions)
        {
            var rootList = new HashSet<GraphNode> { rootNode };
            var toVisit = new Stack<(HashSet<GraphNode> parentList, GraphNode node)>();
            toVisit.Push((rootList, rootNode));

            var visitedNodes = new HashSet<GraphNode>();

            while (toVisit.Count > 0)
            {
                var current = toVisit.Pop();

                if (allowedInstructions.Contains(current.node.Instruction))
                {
                    if (visitedNodes.Contains(current.node))
                        continue;

                    visitedNodes.Add(current.node);

                    foreach (var child in current.node.Children)
                        toVisit.Push((current.node.Children, child));
                }
                else
                {
                    current.parentList.Remove(current.node);

                    foreach (var child in current.node.Children)
                    {
                        current.parentList.Add(child);
                        toVisit.Push((current.parentList, child));
                    }
                }
            }

            return rootList;
        }

        public static HashSet<GraphNode> GetBranches(this HashSet<GraphNode> nodes)
        {
            var toVisit = new Stack<GraphNode>(nodes);
            var branches = new HashSet<GraphNode>();
            var visitedNodes = new HashSet<GraphNode>();

            while (toVisit.Count > 0)
            {
                var node = toVisit.Pop();

                if (visitedNodes.Contains(node))
                    continue;

                visitedNodes.Add(node);

                if (node.Children.Count > 1)
                {
                    branches.Add(node);
                }

                foreach (var child in node.Children)
                    toVisit.Push(child);
            }

            return branches;
        }
    }
}
