using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace MiniCover.Utils
{
    public class ParsingUtils
    {
        public static IEnumerable<Hit> Parse(string json)
        {
            var elements = JArray.Parse(json);
            return elements.Cast<JObject>().Select(ParseAnHit).ToArray();
        }

        private static Hit ParseAnHit(JObject jobject)
        {
            jobject.TryGetValue(nameof(Hit.InstructionId), StringComparison.InvariantCultureIgnoreCase, out var id);
            jobject.TryGetValue(nameof(Hit.Counter), StringComparison.InvariantCultureIgnoreCase, out var counter);
            jobject.TryGetValue(nameof(Hit.TestMethods), StringComparison.InvariantCultureIgnoreCase, out var testMethods);
            return Hit.Build(id.ToObject<int>(), counter.ToObject<int>(), testMethods.ToObject<IEnumerable<TestMethodInfo>>());
        }
    }
}