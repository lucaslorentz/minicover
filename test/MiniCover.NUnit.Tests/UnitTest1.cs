using NUnit.Framework;

namespace MiniCover.Tests.NUnit
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void NUnitTest1()
        {
            var a = 2;
            var b = 2;
            var c = a + b;

            Assert.AreEqual(a, b);
            Assert.AreEqual(4, c);

            new AnotherClass().SomeMethod();
        }

        [Test]
        public void NUnitTest2()
        {
            var a = 2;
            var b = 2;

            if (a != b)
            {
                Assert.AreEqual(a, b);
            }

            new AnotherClass().AnotherMethod();
        }
    }
}
