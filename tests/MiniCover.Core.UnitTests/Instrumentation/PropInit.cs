using System.Linq;
using AwesomeAssertions;

namespace MiniCover.Core.UnitTests.Instrumentation
{
    public class PropInit : BaseTest
    {
        public class Class
        {
            public int Value { get; init; }
        }

        public PropInit() : base(typeof(Class).GetProperties().First().GetSetMethod())
        {
        }

        public override void FunctionalTest()
        {
            var result = new Class { Value = 5 };
            result.Value.Should().Be(5);
        }
    }
}
