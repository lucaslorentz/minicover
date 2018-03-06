using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MiniCover
{
    public sealed class Hit
    {
        private ConcurrentDictionary<MethodBase, HitTestMethod> _testMethodsMap = new ConcurrentDictionary<MethodBase, HitTestMethod>();

        private IEnumerable<HitTestMethod> _testMethods;

        public int InstructionId { get; }
        public int Counter { get; private set; }
        public IEnumerable<HitTestMethod> TestMethods => _testMethods ?? _testMethodsMap.Select(kv => kv.Value);

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
            _testMethods = testMethods.ToHashSet();
        }

        public static IList<Hit> MergeDuplicates(IEnumerable<Hit> items)
        {
            return items
                .GroupBy(i => i.InstructionId)
                .Select(g => new Hit(
                    g.Key,
                    g.Sum(h => h.Counter),
                    HitTestMethod.MergeDuplicates(g.SelectMany(hi => hi.TestMethods))
                )).ToArray();
        }

        public void HitedBy(MethodBase testMethod)
        {
            Counter++;

            if (testMethod != null)
            {
                var hitTestMethod = _testMethodsMap.GetOrAdd(testMethod, (t) => new HitTestMethod(t));
                hitTestMethod.Hited();
            }
        }
    }
}