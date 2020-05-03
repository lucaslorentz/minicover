using System;
using FluentAssertions;
using MiniCover.CommandLine.Options;
using MiniCover.Exceptions;
using MiniCover.UnitTests.TestHelpers;
using Xunit;

namespace MiniCover.UnitTests.CommandLine.Options
{
    public class CoverageLoadedFileOptionTests : FileOptionTests<CoverageLoadedFileOption>
    {
        protected override string CurrentDirectory => "/current-directory".ToOSPath();
        protected override string ExpectedDefaultValue => "/current-directory/coverage.json".ToOSPath();
        protected override string InputValue => "folder/file.json".ToOSPath();
        protected override string ExpectedValue => "/current-directory/folder/file.json".ToOSPath();
        protected override string FileContent => "{\"SourcePath\": \"/source-path\"}";

        protected override CoverageLoadedFileOption Create()
        {
            return new CoverageLoadedFileOption(MockFileSystem);
        }

        protected override void VerifyDefaultValue(CoverageLoadedFileOption sut)
        {
            base.VerifyDefaultValue(sut);
            sut.Result.SourcePath.Should().Be("/source-path");
        }

        protected override void VerifyInputValue(CoverageLoadedFileOption sut)
        {
            base.VerifyInputValue(sut);
            sut.Result.SourcePath.Should().Be("/source-path");
        }

        [Fact]
        public void FileDoesntExist_ShouldThrow()
        {
            MockFileSystem.Directory.SetCurrentDirectory(CurrentDirectory);

            var sut = Create();
            Action act = () => sut.ReceiveValue(InputValue);
            act.Should().Throw<ValidationException>();
        }
    }
}
