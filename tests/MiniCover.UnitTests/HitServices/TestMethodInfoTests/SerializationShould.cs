using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

namespace MiniCover.UnitTests.HitServices.TestMethodInfoTests
{
    [TestFixture]
    public class SerializationShould
    {
        [Test]
        public void Return_the_same_object()
        {
            var expected = new TestMethodInfo(typeof(GetCurrentTestMethodInfoShould).Assembly.FullName,
                nameof(GetCurrentTestMethodInfoShould), nameof(Return_the_same_object),
                typeof(GetCurrentTestMethodInfoShould).Assembly.Location);

            var binaryFormatter = new BinaryFormatter();

            
            using (var stream = new MemoryStream())
            {
                binaryFormatter.Serialize(stream, expected);
                stream.Position = 0;
                var method = binaryFormatter.Deserialize(stream);
                Assert.AreEqual(expected, method);
            }
        }
    }
}