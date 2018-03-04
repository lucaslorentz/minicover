using System;
using System.Linq;
using MiniCover.Utils;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace MiniCover.UnitTests.Utils.ParsingUtilsTests
{
    public class SerializationShould
    {
        [Test]
        public void Return_the_same_object()
        {
            var expected = new Hit[]
            {
                Hit.Build(1, 1, new[]
                {
                    TestMethodInfo.Build(typeof(SerializationShould).Assembly.FullName,
                        nameof(SerializationShould), nameof(Return_the_same_object),
                        typeof(SerializationShould).Assembly.Location)
                })
            };
                
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(expected);
            var method = ParsingUtils.Parse(json);

            Assert.AreEqual(expected.First().InstructionId, method.First().InstructionId);
            Assert.AreEqual(expected.First().Counter, method.First().Counter);
            Assert.AreEqual(expected.First().TestMethods, method.First().TestMethods);

        }
    }
}