using MiniCover.Tests;
using Xunit;

namespace MiniCover.XUnit.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void XUnitTest1()
        {
            var a = 2;
            var b = 2;
            var c = a + b;

            Assert.Equal(a, b);
            Assert.Equal(4, c);

            var obj = new AnotherClass();
            obj.SomeProperty = 6;
            obj.SomeMethod();
        }

        [Fact]
        public void XUnitTest2()
        {
            var a = 2;
            var b = 2;

            if (a != b)
            {
                Assert.Equal(a, b);
            }

            for (int i = 0; i < 50; i++)
            {
                new AnotherClass().AnotherMethod();
            }
        }
    }
}
