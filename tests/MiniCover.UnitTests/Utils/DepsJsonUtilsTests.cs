using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using MiniCover.UnitTests.TestHelpers;
using MiniCover.Utils;
using Xunit;

namespace MiniCover.UnitTests.Utils
{
    public class DepsJsonUtilsTests
    {
        private readonly MockFileSystem _mockFileSystem;
        private readonly DepsJsonUtils _sut;

        public DepsJsonUtilsTests()
        {
            _mockFileSystem = new MockFileSystem();

            _sut = new DepsJsonUtils(_mockFileSystem);
        }

        [Fact]
        public void ShouldPatchDepsJson()
        {
            var fileName = @"c:\source\deps.json";
            var originalContent = GetOriginalDepsJsonContent();

            _mockFileSystem.AddFile(fileName, new MockFileData(originalContent));

            var expectedResult = GetPatchedDepsJsonContent();
            _sut.PatchDepsJson(_mockFileSystem.FileInfo.FromFileName(fileName), "1.0.0");

            var result = _mockFileSystem.GetFile(fileName).TextContents;
            result.ToOSLineEnding().Should().Be(expectedResult.ToOSLineEnding());
        }

        [Fact]
        public void ShouldUnpatchDepsJson()
        {
            var fileName = @"c:\source\deps.json";
            var patchedContent = GetPatchedDepsJsonContent();

            var mockFileData = new MockFileData(patchedContent);
            _mockFileSystem.AddFile(fileName, mockFileData);

            var expectedResult = GetOriginalDepsJsonContent();
            _sut.UnpatchDepsJson(_mockFileSystem.FileInfo.FromFileName(fileName));

            var result = _mockFileSystem.GetFile(fileName).TextContents;
            result.ToOSLineEnding().Should().Be(expectedResult.ToOSLineEnding());
        }

        [Fact]
        public void GetAdditionalPaths_EmptyRuntimeConfigContext_NoError()
        {
            var result = _sut.GetAdditionalPaths(string.Empty);
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetAdditionalPaths_NullRuntimeConfigContext_NoError()
        {
            var result = _sut.GetAdditionalPaths("null");
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetAdditionalPaths_EmptyObject_NoError()
        {
            var result = _sut.GetAdditionalPaths(@"{}");
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

            var result = _sut.GetAdditionalPaths(runtimeConfigContent);
            result.Should().NotBeEmpty();
            result.Count.Should().Be(2);
            result.Should().Contain("/api/.nuget/packages");
            result.Should().Contain("/usr/share/dotnet/sdk/NuGetFallbackFolder");
        }

        [Fact]
        public void LoadDependencyContext_NoFile_ReturnsNull()
        {
            var directory = @"c:\test";
            _mockFileSystem.AddDirectory(directory);
            var directoryInfo = _mockFileSystem.DirectoryInfo.FromDirectoryName(directory);
            var result = _sut.LoadDependencyContext(directoryInfo);
            result.Should().BeNull();
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
