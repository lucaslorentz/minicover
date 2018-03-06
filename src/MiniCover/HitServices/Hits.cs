using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            var hits = ReadHitsFromFile(file).ToArray();

            return new Hits(hits);
        }

        private static IEnumerable<Hit> ReadHitsFromFile(string file)
        {
            if (!File.Exists(file))
                return Enumerable.Empty<Hit>();

            var json = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<Hit[]>($"[{json}]");
        }
    }
}