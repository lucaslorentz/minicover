using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

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
        public void NUnitTestAsync()
        {
            Parallel.Invoke(new ParallelOptions{ MaxDegreeOfParallelism = 10 },Enumerable.Range(0, 50).Select<int, Action>(i => (() => new AnotherClass().AMethodNotAsync())).ToArray());
            Task.WaitAll(Enumerable.Range(0, 50).Select(i => new AnotherClass().AMethodAsync()).ToArray());
        }
    }
}
