using System;
using System.Linq;
using System.Threading.Tasks;
using Sample.TryFinally;
using Xunit;

namespace Sample.UnitTests
{
    public class ClassWithGeneratedRegexTest
    {
        [Fact]
        public void Test()
        {
            var isEmail = ClassWithGeneratedRegex.IsEmail("test@test.com");
            Assert.True(isEmail);
        }
    }
}
