using System;
using FluentAssertions;
using MiniCover.Reports.Utils;
using Xunit;

namespace MiniCover.UnitTests.Reports.Utils
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
