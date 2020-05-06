using FluentAssertions;
using MiniCover.CommandLine;
using Xunit;

namespace MiniCover.UnitTests.CommandLine
{
    public abstract class CommandTests<T> : TestBase
        where T : ICommand
    {
        protected T Sut { get; set; }

        [Fact]
        public void ShouldHaveProperties()
        {
            Sut.CommandName.Should().NotBeEmpty();
            Sut.CommandDescription.Should().NotBeEmpty();
            Sut.Options.Should().NotBeNull();
        }
    }
}
