using System;
using System.Linq;
using System.Threading.Tasks;
using Sample.TryFinally;
using Xunit;

namespace Sample.UnitTests
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

            var obj = new AnotherClass();
            obj.SomeProperty = 6;
            obj.SomeMethod();
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

            for (int i = 0; i < 50; i++)
            {
                new AnotherClass().AnotherMethod();
            }
        }

        [Fact]
        public void TestOnCodeWithTryFinally()
        {
            var test = new AClassWithSomeTryFinally();
            var result = test.MultiplyByTwo(2);
            Assert.Equal(4, result);
        }

        [Fact]
        public void TestOnCodeWithoutTryFinally()
        {
            var test = new AnotherClassWithoutTryFinally();
            var result = test.MultiplyByTwo(2);
            Assert.Equal(4, result);
        }

        [Fact]
        public void TestOnSimpleLambda()
        {
            var test = new ClassWithSimpleLambda();
            var result = test.Add2ToEachValueAndSumThem(2, 4);
            Assert.Equal(10, result);
        }

        [Fact]
        public void TestOnComplicatedLambda()
        {
            var test = new ClassWithComplicatedLambda();
            var result = test.Add2ToEachValueAndSumThem(2, 4);
            Assert.Equal(10, result);
        }

        [Fact]
        public void TestAPartialClass()
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

        [Fact]
        public void TestBuilderWithStaticUsage()
        {
            var instance = ClassWithMultipleConstructors.Default();
        }

        [Fact]
        public void TestBuilderWithParameterWithStaticUsage()
        {
            var instance = ClassWithMultipleConstructors.BuildFor(15);
        }

        [Fact]
        public void TestHeritage()
        {
            var instance = new HeritingClass(12);
            Assert.Equal(180, instance.Value);
        }

        [Fact]
        public void XunitYield()
        {
            var instance = new ClassWithYield();
            instance.Execute();
        }

        [Fact]
        public void MultipleReturn()
        {
            var instance = new AnotherClass();
            Assert.Equal(1, instance.AMethodWithMultipleReturn(1));
        }

        [Fact]
        public void EnumParamWithDefault()
        {
            DefaultNullableParam();
            DefaultParam();
        }

        public void DefaultNullableParam(DemoEnum? value = DemoEnum.A)
        {
        }

        public void DefaultParam(DemoEnum value = DemoEnum.A)
        {
        }
    }
}
