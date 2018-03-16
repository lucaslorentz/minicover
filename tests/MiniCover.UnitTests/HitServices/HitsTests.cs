using System.Collections.Generic;
using System.Linq;
using MiniCover.HitServices;
using Shouldly;
using Xunit;

namespace MiniCover.UnitTests.HitServices
{
    public class HitsTests
    {
        [Fact]
        public void ShouldMergeTestsCorrectly()
        {
            var tests = new[]
            {
                new HitTestMethod("MiniCover.XUnit.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                    "MiniCover.XUnit.Tests.UnitTest1", "XUnitTest2",
                    @"C:\\Repos\\minicover\\test\\MiniCover.XUnit.Tests\\bin\\Debug\\netcoreapp2.0\\MiniCover.XUnit.Tests.dll",
                    12500305,
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
                new HitTestMethod("MiniCover.NUnit.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                    "MiniCover.NUnit.Tests.UnitTest1", "NUnitTest2",
                    @"C:\\Repos\\minicover\\test\\MiniCover.NUnit.Tests\\bin\\Debug\\netcoreapp2.0\\MiniCover.NUnit.Tests.dll",
                    12500305,
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
            
            var hits = Hits.ConvertToHits(tests);

            hits.GetInstructionHitCount(17).ShouldBe(5000000);
            hits.GetInstructionTestMethods(17).Count().ShouldBe(2);
            hits.GetInstructionTestMethods(17).First().Counter.ShouldBe(2500000);
        }

        [Fact]
        public void ShouldMergeTests()
        {
            var tests = new[]
            {
                new HitTestMethod("MiniCover.XUnit.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                    "MiniCover.XUnit.Tests.UnitTest1", "XUnitTest2",
                    @"C:\\Repos\\minicover\\test\\MiniCover.XUnit.Tests\\bin\\Debug\\netcoreapp2.0\\MiniCover.XUnit.Tests.dll",
                    1,
                    new Dictionary<int, int>
                    {
                        {8, 1},
                    }),
                new HitTestMethod("MiniCover.XUnit.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                    "MiniCover.XUnit.Tests.UnitTest1", "XUnitTest2",
                    @"C:\\Repos\\minicover\\test\\MiniCover.XUnit.Tests\\bin\\Debug\\netcoreapp2.0\\MiniCover.XUnit.Tests.dll",
                    1,
                    new Dictionary<int, int>
                    {
                        {8, 1},

                    })
            };

            var hits = Hits.ConvertToHits(tests);
            hits.GetInstructionHitCount(8).ShouldBe(2);
            hits.GetInstructionTestMethods(8).ShouldHaveSingleItem();
            hits.GetInstructionTestMethods(8).First().Counter.ShouldBe(2);
        }
    }
}