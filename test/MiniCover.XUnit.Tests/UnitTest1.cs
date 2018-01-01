using System;
using Xunit;

namespace MiniCover.Tests
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

            new AnotherClass().SomeMethod();
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

            new AnotherClass().AnotherMethod();
        }
    }
}
