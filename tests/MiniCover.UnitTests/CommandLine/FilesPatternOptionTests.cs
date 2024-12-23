using FluentAssertions;
using MiniCover.CommandLine.Options;
using Xunit;

namespace MiniCover.UnitTests.CommandLine
{
    public abstract class FilesPatternOptionTests<T>
        where T : FilesPatternOption
    {
        protected abstract string[] ExpectedDefaultValue { get; }
        protected abstract string[] InputValue { get; }
        protected abstract string[] ExpectedValue { get; }

        protected abstract T Create();

        [Fact]
        public void ShouldHaveProperties()
        {
            var sut = Create();
            sut.Name.Should().NotBeEmpty();
            sut.Description.Should().NotBeEmpty();
        }

        [Fact]
        public void NullValue_ShouldReturnDefaultValue()
        {
            var sut = Create();
            sut.ReceiveValue(null);
            sut.Value.Should().NotBeNull();
            sut.Value.Should().BeEquivalentTo(ExpectedDefaultValue);
        }

        [Fact]
        public void EmptyValue_ShouldReturnDefaultValue()
        {
            var sut = Create();
            sut.ReceiveValue(new string[0]);
            sut.Value.Should().NotBeNull();
            sut.Value.Should().BeEquivalentTo(ExpectedDefaultValue);
        }

        [Fact]
        public void Value_ShouldReturnValue()
        {
            var sut = Create();
            sut.ReceiveValue(InputValue);
            sut.Value.Should().NotBeNull();
            sut.Value.Should().BeEquivalentTo(ExpectedValue);
        }
    }
}
