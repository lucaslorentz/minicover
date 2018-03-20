using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using Sample.TryFinally;

namespace Sample.Tests.NUnit
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

        [Test]
        public void NUnitTestOnCodeWithTryFinally()
        {
            var test = new AClassWithSomeTryFinally();
            var result = test.MultiplyByTwo(2);
            Assert.AreEqual(4, result);
        }

        [Test]
        public void NUnitTestOnCodeWithoutTryFinally()
        {
            var test = new AnotherClassWithoutTryFinally();
            var result = test.MultiplyByTwo(2);
            Assert.AreEqual(4, result);
        }

        [Test]
        public void NUnitTestOnSimpleLambda()
        {
            var test = new ClassWithSimpleLambda();
            var result = test.Add2ToEachValueAndSumThem(2, 4);
            Assert.AreEqual(10, result);
        }

        [Test]
        public void NUnitTestOnComplicatedLambda()
        {
            var test = new ClassWithComplicatedLambda();
            var result = test.Add2ToEachValueAndSumThemWithConsoleWrite(2, 4);
            Assert.AreEqual(10, result);
        }

        [Test]
        public void NunitTestAPartialClass()
        {
            var partialClass = new PartialClass(15);
            Assert.Throws<Exception>(() =>  partialClass.CallPartialMethod(), "15");
        }

        [Test]
        public void NUnitTestAsync()
        {
            Task.WaitAll(Enumerable.Range(0, 50).Select(i => new AnotherClass().AMethodAsync()).ToArray());
        }

        [Test]
        public void NUnitTestParallelAsync()
        {
            Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = 10 }, Enumerable.Range(0, 50).Select<int, Action>(i =>new AnotherClass().AMethodNotAsync).ToArray());
        }
    }
}
