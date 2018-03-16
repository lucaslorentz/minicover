using System.Collections.Generic;
using System.IO;
using System.Linq;
using NFluent;
using Xunit;

namespace MiniCover.HitServices.UnitTests
{
    public class HitTestMethodTests
    {
        [Fact]
        public void BinarySerializationShouldWork()
        {
            var assembly = this.GetType().Assembly;
            var sut = new HitTestMethod(assembly.FullName, this.GetType().FullName, nameof(BinarySerializationShouldWork), assembly.Location, 15, new Dictionary<int, int> { { 1, 15 } });

            byte[] data;
            using (var stream = new MemoryStream())
            {
                sut.Serialize(stream);
                data = stream.ToArray();
            }

            using (var stream = new MemoryStream(data))
            {
                var results = HitTestMethod.Deserialize(stream);
                Check.That(results).HasSize(1);
                var result = results.First();
                Check.That(result).Considering().All.Properties.Excluding(nameof(HitTestMethod.HitedInstructions)).Equals(sut);
                Check.That(result.HitedInstructions).ContainsExactly(sut.HitedInstructions);
            }
        }
    }
}