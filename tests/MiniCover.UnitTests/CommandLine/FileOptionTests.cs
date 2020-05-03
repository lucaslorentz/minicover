using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using MiniCover.CommandLine.Options;
using Xunit;

namespace MiniCover.UnitTests.CommandLine
{
    public abstract class FileOptionTests<T>
        where T : FileOption
    {
        protected abstract string CurrentDirectory { get; }
        protected abstract string ExpectedDefaultValue { get; }
        protected abstract string InputValue { get; }
        protected abstract string ExpectedValue { get; }
        protected virtual string FileContent => "file-content";

        protected MockFileSystem MockFileSystem { get; }

        public FileOptionTests()
        {
            MockFileSystem = new MockFileSystem();
        }

        protected abstract T Create();

        [Fact]
        public void ShouldHaveProperties()
        {
            var sut = Create();
            sut.Template.Should().NotBeEmpty();
            sut.Description.Should().NotBeEmpty();
        }

        [Fact]
        public void NoValue_ShouldReturnDefaultValue()
        {
            MockFileSystem.AddFile(ExpectedDefaultValue, FileContent);
            MockFileSystem.Directory.SetCurrentDirectory(CurrentDirectory);

            var sut = Create();
            sut.ReceiveValue(null);
            VerifyDefaultValue(sut);
        }

        [Fact]
        public void Value_ShouldReturnValue()
        {
            MockFileSystem.AddFile(ExpectedValue, FileContent);
            MockFileSystem.Directory.SetCurrentDirectory(CurrentDirectory);

            var sut = Create();
            sut.ReceiveValue(InputValue);
            VerifyInputValue(sut);
        }

        protected virtual void VerifyDefaultValue(T sut)
        {
            sut.FileInfo.Should().NotBeNull();
            sut.FileInfo.FullName.Should().Be(ExpectedDefaultValue);
        }

        protected virtual void VerifyInputValue(T sut)
        {
            sut.FileInfo.Should().NotBeNull();
            sut.FileInfo.FullName.Should().Be(ExpectedValue);
        }
    }
}
