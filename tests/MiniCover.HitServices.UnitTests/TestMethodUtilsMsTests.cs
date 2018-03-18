using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace MiniCover.HitServices.UnitTests
{
    [TestClass]
    public class TestMethodUtilsMsTests
    {
        [TestMethod]
        public void TestMethodUtil_ReturnTestMethod()
        {
            var method = TestMethodUtils.GetTestMethod();
            method.Name.ShouldBe(nameof(TestMethodUtil_ReturnTestMethod));
        }
    }
}