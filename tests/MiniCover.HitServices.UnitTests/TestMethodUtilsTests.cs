using Shouldly;
using Xunit;

namespace MiniCover.HitServices.UnitTests
{
    public class TestMethodUtilsTests
    {
        [Fact]
        public void TestMethodUtil_ReturnTestMethod()
        {
            var method = TestMethodUtils.GetTestMethod();
            method.Name.ShouldBe(nameof(TestMethodUtil_ReturnTestMethod));
        }
    }
}