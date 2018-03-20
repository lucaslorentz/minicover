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
        public void NUnitTestOnSimpleLambda()
        {
            var test = new ClassWithSimpleLambda();
            var result = test.Add2ToEachValueAndSumThem(2, 4);
            Assert.Equal(10, result);
        }

        [Fact]
        public void NUnitTestOnComplicatedLambda()
        {
            var test = new ClassWithComplicatedLambda();
            var result = test.Add2ToEachValueAndSumThemWithConsoleWrite(2, 4);
            Assert.Equal(10, result);
        }

        [Fact]
        public void NunitTestAPartialClass()
        {
            var partialClass = new PartialClass(15);
            Assert.Throws<Exception>(() => partialClass.CallPartialMethod());
        }

        [Fact]
        public void XUnitTestAsync()
        {
            Task.WaitAll(Enumerable.Range(0, 50).Select(i => new AnotherClass().AMethodAsync()).ToArray());
        }

        [Fact]
        public void XUnitTestParallelAsync()
        {
            var tasks = Enumerable.Range(0, 50).Select<int, Action>(i => new AnotherClass().AMethodNotAsync)
                .ToArray();
            Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = 10 }, tasks);
        }
    }
}
