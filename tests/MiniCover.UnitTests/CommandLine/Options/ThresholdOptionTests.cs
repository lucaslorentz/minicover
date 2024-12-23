using FluentAssertions;
using MiniCover.CommandLine.Options;
using Xunit;

namespace MiniCover.UnitTests.CommandLine.Options
{
    public class ThresholdOptionTests
    {
        [Fact]
        public void ShouldHaveProperties()
        {
            var sut = new ThresholdOption();
            sut.Name.Should().NotBeEmpty();
            sut.Description.Should().NotBeEmpty();
        }

        [Fact]
        public void NoValue_ShouldReturnDefaultValue()
        {
            var sut = new ThresholdOption();
            sut.ReceiveValue(null);
            sut.Value.Should().Be(0.9f);
        }

        [Fact]
        public void Value_ShouldReturnValue()
        {
            var sut = new ThresholdOption();
            sut.ReceiveValue("80.51");
            sut.Value.Should().Be(0.8051f);
        }
    }
}
