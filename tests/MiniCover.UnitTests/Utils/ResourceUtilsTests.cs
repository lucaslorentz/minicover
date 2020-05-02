using System;
using FluentAssertions;
using MiniCover.Utils;
using Xunit;

namespace MiniCover.UnitTests.Utils
{
    public class ResourceUtilsTests
    {
        [Fact]
        public void GetContent()
        {
            var content = ResourceUtils.GetContent("MiniCover.Reports.Html.Shared.css");
            content.Should().NotBeEmpty();
        }
    }
}
