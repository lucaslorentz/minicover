using NUnit.Framework;

namespace MiniCover.UnitTests.HitServices.TestMethodInfoTests
{
    [TestFixture]
    public class GetCurrentTestMethodInfoShould
    {
        [Test]
        public void Return_this_test_method_method_info()
        {
            var method = TestMethodInfo.GetCurrentTestMethodInfo();
            var expected = new TestMethodInfo(typeof(GetCurrentTestMethodInfoShould).Assembly.FullName,
                nameof(GetCurrentTestMethodInfoShould), nameof(Return_this_test_method_method_info),
                typeof(GetCurrentTestMethodInfoShould).Assembly.Location);
            Assert.AreEqual(expected, method);
        }
    }
}