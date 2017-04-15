using System;
using Xunit;

namespace Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var a = 2;
            var b = 2;
            var c = a + b;

            Assert.Equal(a, b);
            Assert.Equal(4, c);

            new AnotherClass().SomeMethod();
        }

        [Fact]
        public void Test2()
        {
            var a = 2;
            var b = 2;

            if (a != b)
            {
                Assert.Equal(a, b);
            }

            new AnotherClass().SomeMethod();
        }
    }
}
