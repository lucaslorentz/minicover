using System.Collections.Generic;

namespace MiniCover.Core.Model
{
    public class GraphNode<T>
    {
        public T Value { get; }

        public HashSet<GraphNode<T>> Children { get; }

        public GraphNode(T value, params GraphNode<T>[] children)
        {
            Value = value;
            Children = new HashSet<GraphNode<T>>(children);
        }
    }
}
