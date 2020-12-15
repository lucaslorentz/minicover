using System.Collections.Generic;
using MiniCover.Core.Model;

namespace MiniCover.Core.Extensions
{
    public static class GraphNodeExtensions
    {
        public static HashSet<GraphNode<T>> Filter<T>(
            this GraphNode<T> rootNode,
            HashSet<T> allowedValues)
        {
            var rootList = new HashSet<GraphNode<T>> { rootNode };

            var pending = new Queue<(HashSet<GraphNode<T>> parentList, GraphNode<T> node)>();
            pending.Enqueue((rootList, rootNode));

            var visitedNodes = new HashSet<(HashSet<GraphNode<T>> parentList, GraphNode<T> node)>();

            while (pending.Count > 0)
            {
                var current = pending.Dequeue();

                if (allowedValues.Contains(current.node.Value))
                {
                    if (!visitedNodes.Add(current))
                        continue;

                    foreach (var child in current.node.Children)
                        pending.Enqueue((current.node.Children, child));
                }
                else if (current.parentList.Remove(current.node))
                {
                    if (!visitedNodes.Add(current))
                        continue;

                    foreach (var child in current.node.Children)
                    {
                        current.parentList.Add(child);
                        pending.Enqueue((current.parentList, child));
                    }
                }
            }

            return rootList;
        }

        public static HashSet<GraphNode<T>> GetBranches<T>(this HashSet<GraphNode<T>> nodes)
        {
            var pending = new Queue<GraphNode<T>>(nodes);
            var branches = new HashSet<GraphNode<T>>();
            var visitedNodes = new HashSet<GraphNode<T>>();

            while (pending.Count > 0)
            {
                var node = pending.Dequeue();

                if (!visitedNodes.Add(node))
                    continue;

                if (node.Children.Count > 1)
                {
                    branches.Add(node);
                }

                foreach (var child in node.Children)
                    pending.Enqueue(child);
            }

            return branches;
        }
    }
}
