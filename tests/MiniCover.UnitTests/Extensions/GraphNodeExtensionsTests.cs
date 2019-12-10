using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using MiniCover.Extensions;
using MiniCover.Model;
using Xunit;

namespace MiniCover.UnitTests.Extensions
{
    public class GraphNodeExtensionsTests
    {
        [Fact]
        public void Filter_KeepCyclicReferences()
        {
            GraphNode<int> node2;
            GraphNode<int> node3;

            var node1 = new GraphNode<int>(1,
                node2 = new GraphNode<int>(2,
                    node3 = new GraphNode<int>(3)
                )
            );
            node3.Children.Add(node2);

            var filtered = node1.Filter(new HashSet<int> { 1, 2, 3 });

            Format(filtered).Should().Be("1[2[3[*2]]]");
        }

        [Fact]
        public void Filter_RemoveCyclicReferences()
        {
            GraphNode<int> node2;
            GraphNode<int> node3;

            var node1 = new GraphNode<int>(1,
                node2 = new GraphNode<int>(2,
                    node3 = new GraphNode<int>(3)
                )
            );
            node3.Children.Add(node2);

            var filtered = node1.Filter(new HashSet<int> { 1 });

            Format(filtered).Should().Be("1");
        }

        [Fact]
        public void Filter_KeepSelfReferenceNested()
        {
            GraphNode<int> node2;

            var node1 = new GraphNode<int>(1,
                node2 = new GraphNode<int>(2)
            );
            node2.Children.Add(node2);

            var filtered = node1.Filter(new HashSet<int> { 1, 2 });

            Format(filtered).Should().Be("1[2[*2]]");
        }

        [Fact]
        public void Filter_RemoveSelfReferenceNested()
        {
            GraphNode<int> node2;

            var node1 = new GraphNode<int>(1,
                node2 = new GraphNode<int>(2)
            );
            node2.Children.Add(node2);

            var filtered = node1.Filter(new HashSet<int> { 1 });

            Format(filtered).Should().Be("1");
        }

        [Fact]
        public void Filter_RemoveRootNode()
        {
            var node1 = new GraphNode<int>(1,
                new GraphNode<int>(2),
                new GraphNode<int>(3)
            );

            var filtered = node1.Filter(new HashSet<int> { 2, 3 });

            Format(filtered).Should().Be("2,3");
        }

        [Fact]
        public void Filter_ComplexCase()
        {
            GraphNode<int> node2;
            GraphNode<int> node3;
            GraphNode<int> node4;
            GraphNode<int> node5;
            GraphNode<int> node6;
            GraphNode<int> node7;
            GraphNode<int> node8;

            var node1 = new GraphNode<int>(1,
                node2 = new GraphNode<int>(2),
                node3 = new GraphNode<int>(3,
                    node4 = new GraphNode<int>(4),
                    node5 = new GraphNode<int>(5),
                    node6 = new GraphNode<int>(6,
                        node7 = new GraphNode<int>(7),
                        node8 = new GraphNode<int>(8)
                    )
                )
            );
            node2.Children.Add(node1);
            node2.Children.Add(node7);
            node2.Children.Add(node8);
            node4.Children.Add(node5);
            node5.Children.Add(node4);
            node6.Children.Add(node6);
            node6.Children.Add(node3);

            var filtered = node1.Filter(new HashSet<int> { 1, 2, 3, 8 });

            Format(filtered).Should().Be("1[2[*1,8],3[*8,*3]]");
        }

        [Fact]
        public void Filter_AvoidInfiniteRecursion1()
        {
            var node4 = new GraphNode<int>(4,
                new GraphNode<int>(5));

            var node1 = new GraphNode<int>(1,
                new GraphNode<int>(2,
                    node4
                ),
                new GraphNode<int>(3,
                    node4
                )
            );

            var filtered = node1.Filter(new HashSet<int> { 1, 5 });

            Format(filtered).Should().Be("1[5]");
        }

        [Fact]
        public void Filter_AvoidInfiniteRecursion2()
        {
            var node5 = new GraphNode<int>(5,
                new GraphNode<int>(6));

            var node1 = new GraphNode<int>(1,
                new GraphNode<int>(2,
                    node5
                ),
                new GraphNode<int>(3,
                    new GraphNode<int>(4,
                        node5
                    )
                )
            );

            var filtered = node1.Filter(new HashSet<int> { 1, 2, 3, 6 });

            Format(filtered).Should().Be("1[2[6],3[*6]]");
        }

        private static string Format<T>(HashSet<GraphNode<T>> nodes)
        {
            var visitedNodes = new HashSet<GraphNode<T>>();

            var result = new StringBuilder();

            FormatList(nodes);

            return result.ToString();

            void FormatList(HashSet<GraphNode<T>> list)
            {
                var first = true;
                foreach (var n in list)
                {
                    if (!first)
                        result.Append(",");

                    first = false;

                    if (visitedNodes.Add(n))
                    {
                        result.Append(n.Value);
                        if (n.Children.Count > 0)
                        {
                            result.Append("[");
                            FormatList(n.Children);
                            result.Append("]");
                        }
                    }
                    else
                    {
                        result.Append("*");
                        result.Append(n.Value);
                    }
                }
            }
        }
    }
}
