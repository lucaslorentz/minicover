using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Shouldly;
using Xunit;

namespace MiniCover.UnitTests.HitServices
{
    public class HitsTests
    {
        [Fact]
        public void ShouldParseTheHitJsonCorrectly()
        {
            var json = @"[
    {
        ""AssemblyName"": ""MiniCover.XUnit.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"",
        ""ClassName"": ""UnitTest1"",
        ""MethodName"": ""XUnitTest2"",
        ""AssemblyLocation"": ""C:\\Users\\bhugo_000\\Source\\Repos\\minicover\\test\\MiniCover.XUnit.Tests\\bin\\Debug\\netcoreapp2.0\\MiniCover.XUnit.Tests.dll"",
        ""Counter"": 12500305,
        ""HitedInstructions"": {
            ""17"": 2500000,
            ""19"": 2500000,
            ""20"": 50,
            ""21"": 2500000,
            ""22"": 2500000,
            ""23"": 2500050,
            ""24"": 50,
            ""33"": 1,
            ""34"": 1,
            ""35"": 1,
            ""37"": 1,
            ""38"": 50,
            ""39"": 50,
            ""40"": 51
        }
    },
    {
        ""AssemblyName"": ""MiniCover.NUnit.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"",
        ""ClassName"": ""UnitTest1"",
        ""MethodName"": ""NUnitTest2"",
        ""AssemblyLocation"": ""C:\\Users\\bhugo_000\\Source\\Repos\\minicover\\test\\MiniCover.NUnit.Tests\\bin\\Debug\\netcoreapp2.0\\MiniCover.NUnit.Tests.dll"",
        ""Counter"": 12500305,
        ""HitedInstructions"": {
            ""9"": 1,
            ""10"": 1,
            ""11"": 1,
            ""13"": 1,
            ""14"": 50,
            ""15"": 50,
            ""16"": 51,
            ""17"": 2500000,
            ""19"": 2500000,
            ""20"": 50,
            ""21"": 2500000,
            ""22"": 2500000,
            ""23"": 2500050,
            ""24"": 50
        }
    }
]";

            var hits = Hits.ConvertToHits(json);

            hits.GetInstructionHitCount(17).ShouldBe(5000000);
            hits.GetInstructionTestMethods(17).Count().ShouldBe(2);
            hits.GetInstructionTestMethods(17).First().Counter.ShouldBe(2500000);
        }

        [Fact]
        public void ShouldMergeTests()
        {
            var json = @"[{
        ""AssemblyName"": ""MiniCover.XUnit.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"",
        ""ClassName"": ""UnitTest1"",
        ""MethodName"": ""XUnitTest2"",
        ""AssemblyLocation"": ""C:\\Users\\bhugo_000\\Source\\Repos\\minicover\\test\\MiniCover.XUnit.Tests\\bin\\Debug\\netcoreapp2.0\\MiniCover.XUnit.Tests.dll"",
        ""Counter"": 1,
        ""HitedInstructions"": {
            ""8"": 1
        }
    },
    {
        ""AssemblyName"": ""MiniCover.XUnit.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"",
        ""ClassName"": ""UnitTest1"",
        ""MethodName"": ""XUnitTest2"",
        ""AssemblyLocation"": ""C:\\Users\\bhugo_000\\Source\\Repos\\minicover\\test\\MiniCover.XUnit.Tests\\bin\\Debug\\netcoreapp2.0\\MiniCover.XUnit.Tests.dll"",
        ""Counter"": 1,
        ""HitedInstructions"": {
            ""8"": 1
        }
    }]";

            var hits = Hits.ConvertToHits(json);
            hits.GetInstructionHitCount(8).ShouldBe(2);
            hits.GetInstructionTestMethods(8).ShouldHaveSingleItem();
            hits.GetInstructionTestMethods(8).First().Counter.ShouldBe(2);
        }
    }

   
}