using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MiniCover.HitServices;
using Xunit;

namespace MiniCover.UnitTests.HitServices
{
    public class HitsInfoTests
    {
        [Fact]
        public void ShouldMergeTestsCorrectly()
        {
            var contexts = new[]
            {
                new HitContext(
                    "Sample.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                    "Sample.UnitTests.UnitTest1",
                    "XUnitTest2",
                    new Dictionary<int, int>
                    {
                        {17, 2500000},
                        {19, 2500000},
                        {20, 50},
                        {21, 2500000},
                        {22, 2500000},
                        {23, 2500050},
                        {24, 50},
                        {33, 1},
                        {34, 1},
                        {35, 1},
                        {37, 1},
                        {38, 50},
                        {39, 50},
                        {40, 51}
                    }),
                new HitContext(
                    "Sample.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                    "Sample.UnitTests.UnitTest1", "NUnitTest2",
                    new Dictionary<int, int>
                    {
                        {9, 1},
                        {10, 1},
                        {11, 1},
                        {13, 1},
                        {14, 50},
                        {15, 50},
                        {16, 51},
                        {17, 2500000},
                        {19, 2500000},
                        {20, 50},
                        {21, 2500000},
                        {22, 2500000},
                        {23, 2500050},
                        {24, 50}
                    })
            };
            
            var hits = new HitsInfo(contexts);

            hits.GetInstructionHitCount(17).Should().Be(5000000);
            hits.GetInstructionHitContexts(17).Count().Should().Be(2);
            hits.GetInstructionHitContexts(17).First().GetHitCount(17).Should().Be(2500000);
        }

        [Fact]
        public void ShouldMergeTests()
        {
            var contexts = new[]
            {
                new HitContext(
                    "Sample.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                    "Sample.UnitTests.UnitTest1",
                    "XUnitTest2",
                    new Dictionary<int, int>
                    {
                        {8, 1},
                    }),
                new HitContext(
                    "Sample.UnitTests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                    "Sample.UnitTests.UnitTest1",
                    "XUnitTest2",
                    new Dictionary<int, int>
                    {
                        {8, 1},
                    })
            };

            var hits = new HitsInfo(contexts);
            hits.GetInstructionHitCount(8).Should().Be(2);
            hits.GetInstructionHitContexts(8).Should().HaveCount(1);
            hits.GetInstructionHitContexts(8).First().GetHitCount(8).Should().Be(2);
        }
    }
}