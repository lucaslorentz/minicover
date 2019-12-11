using System.Collections.Generic;
using System.IO;
using System.Linq;
using MiniCover.HitServices;

namespace MiniCover
{
    public class HitsInfo
    {
        private readonly Dictionary<int, HitValues> _valuesById;

        public HitsInfo(IEnumerable<HitContext> contexts)
        {
            _valuesById = HitContext.MergeDuplicates(contexts)
                .SelectMany(c => c.Hits.Keys, (context, id) => new {
                    context,
                    id,
                    hitCount = context.GetHitCount(id)
                })
                .GroupBy(g => g.id)
                .ToDictionary(g => g.Key, g => new HitValues
                {
                    HitCount = g.Sum(d => d.hitCount),
                    Contexts = g.Select(d => d.context).ToArray()
                });
        }

        public bool WasHit(int id)
        {
            return _valuesById.ContainsKey(id);
        }

        public int GetHitCount(int id)
        {
            if (!_valuesById.TryGetValue(id, out var values))
                return 0;

            return values.HitCount;
        }

        public IEnumerable<HitContext> GetHitContexts(int id)
        {
            if (!_valuesById.TryGetValue(id, out var values))
                return Enumerable.Empty<HitContext>();

            return values.Contexts;
        }

        public static HitsInfo TryReadFromDirectory(string path)
        {
            var contexts = new List<HitContext>();

            if (Directory.Exists(path))
            {
                foreach (var hitFile in Directory.GetFiles(path, "*.hits"))
                {
                    using (var fileStream = File.Open(hitFile, FileMode.Open, FileAccess.Read))
                    {
                        contexts.AddRange(HitContext.Deserialize(fileStream));
                    }
                }
            }

            return new HitsInfo(contexts);
        }

        class HitValues
        {
            public int HitCount { get; set; }
            public HitContext[] Contexts { get; set; }
        }
    }
}