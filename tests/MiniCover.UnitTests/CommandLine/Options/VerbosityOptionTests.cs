using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MiniCover.CommandLine.Options;
using MiniCover.Exceptions;
using MiniCover.Infrastructure;
using Moq;
using Xunit;

namespace MiniCover.UnitTests.CommandLine.Options
{
    public class VerbosityOptionTests
    {
        [Fact]
        public void ShouldHaveProperties()
        {
            var outputMock = new Mock<IOutput>();
            var sut = new VerbosityOption(outputMock.Object);
            sut.Template.Should().NotBeEmpty();
            sut.Description.Should().NotBeEmpty();
        }

        [Fact]
        public void NoValue_ShouldNotCHangeOutput()
        {
            var outputMock = new Mock<IOutput>();
            outputMock.SetupProperty(o => o.MinimumLevel, LogLevel.Information);

            var sut = new VerbosityOption(outputMock.Object);
            sut.ReceiveValue(null);
            outputMock.Object.MinimumLevel.Should().Be(LogLevel.Information);
        }

        [InlineData("debug")]
        [InlineData("Debug")]
        [Theory]
        public void Value_ShouldSetOutputVerbose(string input)
        {
            var outputMock = new Mock<IOutput>();
            outputMock.SetupProperty(o => o.MinimumLevel, LogLevel.Information);

            var sut = new VerbosityOption(outputMock.Object);
            sut.ReceiveValue(input);
            outputMock.Object.MinimumLevel.Should().Be(LogLevel.Debug);
        }

        [Fact]
        public void InvalidValue_ShouldThrow()
        {
            var outputMock = new Mock<IOutput>();
            outputMock.SetupProperty(o => o.MinimumLevel, LogLevel.Information);

            var sut = new VerbosityOption(outputMock.Object);

            Action act = () => sut.ReceiveValue("InvalidLevel");

            act.Should().Throw<ValidationException>();
        }
    }
}
