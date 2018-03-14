using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MiniCover
{
    public sealed class Hit
    {
        private readonly ConcurrentDictionary<MethodBase, HitTestMethod> testMethodsMap = new ConcurrentDictionary<MethodBase, HitTestMethod>();

        private readonly IEnumerable<HitTestMethod> testMethods;

        public int InstructionId { get; }
        public int Counter { get; private set; }
        public IEnumerable<HitTestMethod> TestMethods => testMethods ?? testMethodsMap.Select(kv => kv.Value);

        internal Hit(int instructionId)
        {
            InstructionId = instructionId;
            Counter = 0;
        }

        [JsonConstructor]
        internal Hit(int instructionId, int counter, IEnumerable<HitTestMethod> testMethods)
        {
            InstructionId = instructionId;
            Counter = counter;
            this.testMethods = testMethods.ToHashSet();
        }

        public static IList<Hit> MergeDuplicates(IEnumerable<Hit> items)
        {
            return items
                .GroupBy(i => i.InstructionId)
                .Select(g => Convert(g.Key, g)).ToArray();
        }

        private static Hit Convert(int id, IEnumerable<Hit> hits)
        {
            var items = hits.ToArray();
            var methods = items.SelectMany(hi =>
                hi.TestMethods);
            var validMethods = methods.Where(a => a.HasBeenHitBy(id));
            var converted = HitTestMethod.MergeDuplicates(validMethods, id).ToArray();
            return new Hit(
                id,
                items.Sum(h => h.Counter),
                converted);
        }

        public void HitedBy(MethodBase testMethod)
        {
            Counter++;

            if (testMethod != null)
            {
                var hitTestMethod = testMethodsMap.GetOrAdd(testMethod, (t) => new HitTestMethod(t));
                hitTestMethod.Hited();
            }
        }
    }
}