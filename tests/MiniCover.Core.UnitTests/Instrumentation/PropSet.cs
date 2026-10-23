using System.Linq;
using AwesomeAssertions;

namespace MiniCover.Core.UnitTests.Instrumentation
{
    public class PropSet : BaseTest
    {
        public class Class
        {
            public int Value { get; set; }
        }

        public PropSet() : base(typeof(Class).GetProperties().First().GetSetMethod())
        {
        }

        public override void FunctionalTest()
        {
            var result = new Class { Value = 5 };
            result.Value.Should().Be(5);
        }
    }
}
