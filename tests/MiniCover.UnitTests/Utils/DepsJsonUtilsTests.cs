using FluentAssertions;
using MiniCover.Utils;
using Xunit;

namespace MiniCover.UnitTests.Utils
{
    public class DepsJsonUtilsTests
    {
        [Fact]
        public void ShouldPatchDepsJson()
        {
            var originalContent = GetOriginalDepsJsonContent();
            var expectedResult = GetPatchedDepsJsonContent();
            var result = DepsJsonUtils.PatchDepsJsonContent(originalContent, "1.0.0");
            NormalizeLineEnding(result).Should().Be(NormalizeLineEnding(expectedResult));
        }

        [Fact]
        public void ShouldUnpatchDepsJson()
        {
            var patchedContent = GetPatchedDepsJsonContent();
            var expectedResult = GetOriginalDepsJsonContent();
            var result = DepsJsonUtils.UnpatchDepsJsonContent(patchedContent);
            NormalizeLineEnding(result).Should().Be(NormalizeLineEnding(expectedResult));
        }

        [Fact]
        public void GetAdditionalPaths_EmptyRuntimeConfigContext_NoError()
        {
            var result = DepsJsonUtils.GetAdditionalPaths(string.Empty);
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetAdditionalPaths_EmptyObject_NoError()
        {
            var result = DepsJsonUtils.GetAdditionalPaths(@"{}");
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetAdditionalPaths_RuntimeConfigContext_PathsReturned()
        {
            var runtimeConfigContent = @"{
  ""runtimeOptions"": {
    ""additionalProbingPaths"": [
      ""/root/.dotnet/store/|arch|/|tfm|"",
      ""/api/.nuget/packages"",
      ""/usr/share/dotnet/sdk/NuGetFallbackFolder""
    ]
    }
}";

            var result = DepsJsonUtils.GetAdditionalPaths(runtimeConfigContent);
            result.Should().NotBeEmpty();
            result.Count.Should().Be(2);
            result.Should().Contain("/api/.nuget/packages");
            result.Should().Contain("/usr/share/dotnet/sdk/NuGetFallbackFolder");
        }

        public string NormalizeLineEnding(string text)
        {
            return text.Replace("\r\n", "\n");
        }

        private static string GetOriginalDepsJsonContent()
        {
            return @"{
  ""runtimeTarget"": {
    ""name"": "".NETCoreApp,Version=v2.0"",
    ""signature"": ""da39a3ee5e6b4b0d3255bfef95601890afd80709""
  },
  ""compilationOptions"": {},
  ""targets"": {
    "".NETCoreApp,Version=v2.0"": {
      ""Sample/1.0.0"": {
        ""runtime"": {
          ""Sample.dll"": {}
        }
      }
    }
  },
  ""libraries"": {
    ""Sample/1.0.0"": {
      ""type"": ""project"",
      ""serviceable"": false,
      ""sha512"": """"
    }
  }
}";
        }

        private static string GetPatchedDepsJsonContent()
        {
            return @"{
  ""runtimeTarget"": {
    ""name"": "".NETCoreApp,Version=v2.0"",
    ""signature"": ""da39a3ee5e6b4b0d3255bfef95601890afd80709""
  },
  ""compilationOptions"": {},
  ""targets"": {
    "".NETCoreApp,Version=v2.0"": {
      ""Sample/1.0.0"": {
        ""runtime"": {
          ""Sample.dll"": {}
        }
      },
      ""MiniCover.HitServices/1.0.0"": {
        ""runtime"": {
          ""MiniCover.HitServices.dll"": {}
        }
      }
    }
  },
  ""libraries"": {
    ""Sample/1.0.0"": {
      ""type"": ""project"",
      ""serviceable"": false,
      ""sha512"": """"
    },
    ""MiniCover.HitServices/1.0.0"": {
      ""type"": ""project"",
      ""serviceable"": false,
      ""sha512"": """"
    }
  }
}";
        }
    }
}
