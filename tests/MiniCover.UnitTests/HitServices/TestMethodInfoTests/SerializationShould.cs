using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace MiniCover.UnitTests.HitServices.TestMethodInfoTests
{
    [TestFixture]
    public class SerializationShould
    {
        [Test]
        public void Return_the_same_object()
        {
            var expected = TestMethodInfo.Build(typeof(GetCurrentTestMethodInfoShould).Assembly.FullName,
                nameof(GetCurrentTestMethodInfoShould), nameof(Return_the_same_object),
                typeof(GetCurrentTestMethodInfoShould).Assembly.Location);
            expected.HasCall();
            expected.HasCall();
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(expected);

            var jObject = JObject.Parse(json);
            var method = Parse(jObject);
            Assert.AreEqual(expected, method);
            Assert.AreEqual(expected.Counter, method.Counter);
        }

        private TestMethodInfo Parse(JObject jObject)
        {
            return jObject.ToObject<TestMethodInfo>();
        }

        
    }
}