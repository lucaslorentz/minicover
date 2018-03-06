using MiniCover.Tests;
using NUnit.Framework;

namespace MiniCover.NUnit.Tests
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

            var obj = new AnotherClass();
            obj.SomeProperty = 6;
            obj.SomeMethod();
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

            for (int i = 0; i < 50; i++)
            {
                new AnotherClass().AnotherMethod();
            }
        }
    }
}
