using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace MiniCover.HitServices.UnitTests
{
    public class HitContextTests
    {
        [Fact]
        public void BinarySerializationShouldWork()
        {
            var assembly = this.GetType().Assembly;

            var sut = new HitContext(assembly.FullName, this.GetType().FullName, nameof(BinarySerializationShouldWork), new Dictionary<int, int> { { 1, 15 } });

            byte[] data;
            using (var stream = new MemoryStream())
            {
                sut.Serialize(stream);
                data = stream.ToArray();
            }

            using (var stream = new MemoryStream(data))
            {
                var results = HitContext.Deserialize(stream).ToArray();
                results.Length.Should().Be(1);

                var result = results.First();
                result.ClassName.Should().Be(sut.ClassName);
                result.MethodName.Should().Be(sut.MethodName);
                result.AssemblyName.Should().Be(sut.AssemblyName);
                result.Hits.Should().BeEquivalentTo(sut.Hits);
            }
        }
    }
}