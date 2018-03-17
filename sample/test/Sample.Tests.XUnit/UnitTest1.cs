using System;
using System.Linq;
using System.Threading.Tasks;
using Sample.TryFinally;
using Xunit;

namespace Sample.Tests.XUnit
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

        [Fact]
        public void NUnitTestOnCodeWithTryFinally()
        {
            var test = new AClassWithSomeTryFinally();
            var result = test.MultiplyByTwo(2);
            Assert.Equal(4, result);
        }

        [Fact]
        public void NUnitTestOnCodeWithoutTryFinally()
        {
            var test = new AnotherClassWithoutTryFinally();
            var result = test.MultiplyByTwo(2);
            Assert.Equal(4, result);
        }

        [Fact]
        public void XUnitTestAsync()
        {
            Task.WaitAll(Enumerable.Range(0, 50).Select(i => new AnotherClass().AMethodAsync()).ToArray());
        }

        [Fact]
        public void XUnitTestParallelAsync()
        {
            Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = 10 }, Enumerable.Range(0, 50).Select<int, Action>(i => (() => new AnotherClass().AMethodNotAsync())).ToArray());
        }
    }
}
