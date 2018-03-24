using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace MiniCover.HitServices.UnitTests
{
    public class HitTestMethodTests
    {
        [Fact]
        public void BinarySerializationShouldWork()
        {
            var assembly = this.GetType().Assembly;

            //var sut = new HitTestMethod(assembly.FullName, this.GetType().FullName, nameof(BinarySerializationShouldWork), assembly.Location, 15, new Dictionary<int, int> { { 1, 15 } });
            var currentUri = new Uri(new Uri(Directory.GetCurrentDirectory()),@"../../../../");
            var sut = HitTestMethod.From(TestMethodUtils.GetTestMethod(), currentUri);
            byte[] data;
            using (var stream = new MemoryStream())
            {
                sut.Serialize(stream);
                data = stream.ToArray();
            }

            using (var stream = new MemoryStream(data))
            {
                var results = HitTestMethod.Deserialize(stream).ToArray();
                results.Length.ShouldBe(1);

                var result = results.First();
                result.ClassName.ShouldBe(sut.ClassName);
                result.MethodName.ShouldBe(sut.MethodName);
                result.AssemblyName.ShouldBe(sut.AssemblyName);
                result.AssemblyLocation.ShouldBe(sut.AssemblyLocation);
                result.Counter.ShouldBe(sut.Counter);
                result.HitedInstructions.ShouldBe(sut.HitedInstructions);
            }
        }
    }
}