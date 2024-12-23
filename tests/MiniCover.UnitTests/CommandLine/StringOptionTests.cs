using FluentAssertions;
using MiniCover.CommandLine;
using Xunit;

namespace MiniCover.UnitTests.CommandLine
{
    public class StringOptionTests
    {
        [Fact]
        public void ShouldHaveProperties()
        {
            var sut = Create();
            sut.Name.Should().Be("--template");
            sut.ShortName.Should().Be("-t");
            sut.Description.Should().Be("Description");
        }

        [Fact]
        public void NullValue_ShouldReturnDefaultValue()
        {
            var sut = Create();
            sut.ReceiveValue(null);
            sut.Value.Should().BeNull();
        }

        [Fact]
        public void Value_ShouldReturnValue()
        {
            var sut = Create();
            sut.ReceiveValue("Something");
            sut.Value.Should().Be("Something");
        }

        private static StringOption Create()
        {
            return new StringOption("--template", "Description")
            {
                ShortName = "-t"
            };
        }
    }
}
