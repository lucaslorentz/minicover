using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace MiniCover
{
    public sealed class Hit
    {
        public int InstructionId { get; }
        public int Counter { get; }
        public IEnumerable<HitTestMethod> TestMethods { get; }

        [JsonConstructor]
        internal Hit(int instructionId, int counter, IEnumerable<HitTestMethod> testMethods)
        {
            this.InstructionId = instructionId;
            this.Counter = counter;
            this.TestMethods = testMethods.ToHashSet();
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
    }
}