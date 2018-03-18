using NUnit.Framework;
using Shouldly;

namespace MiniCover.HitServices.UnitTests
{
    public class TestMethodUtilsNunitTests
    {
        [Test]
        public void TestMethodUtil_ReturnTestMethod()
        {
            var method = TestMethodUtils.GetTestMethod();
            method.Name.ShouldBe(nameof(TestMethodUtil_ReturnTestMethod));
        }
    }
}