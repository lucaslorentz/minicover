using System.Collections.Generic;
using System.IO;
using System.Linq;
using MiniCover.HitServices;

namespace MiniCover
{
    public class Hits
    {
        private readonly Dictionary<int, Hit> hits;

        internal Hits(IEnumerable<Hit> hits)
        {
            this.hits = Hit.MergeDuplicates(hits).ToDictionary(h => h.InstructionId);
        }

        public bool IsInstructionHit(int id)
        {
            return hits.ContainsKey(id);
        }

        public int GetInstructionHitCount(int instructionId)
        {
            if (!hits.TryGetValue(instructionId, out var hit))
                return 0;

            return hit.Counter;
        }

        public IEnumerable<HitTestMethod> GetInstructionTestMethods(int instructionId)
        {
            if (!hits.TryGetValue(instructionId, out var hit))
                return Enumerable.Empty<HitTestMethod>();

            return hit.TestMethods;
        }

        public static Hits TryReadFromFile(string file)
        {
            if (!File.Exists(file))
                return new Hits(Enumerable.Empty<Hit>());
            
            using (var fileStream = File.Open(file, FileMode.Open, FileAccess.Read))
            {
                var tests = HitTestMethod.Deserialize(fileStream);
                return ConvertToHits(tests);
            }
        }

        public static Hits ConvertToHits(IEnumerable<HitTestMethod> tests)
        {
            var notMergedHits = tests
                .SelectMany(method => method.HitedInstructions.Select(id => new Hit(id.Key, id.Value, new[] {method}))).ToArray();
            return new Hits(notMergedHits);
        }
    }
}