using System.Collections.Generic;
using System.IO;
using System.Linq;
using MiniCover.HitServices;

namespace MiniCover
{
    public class HitsInfo
    {
        private readonly Dictionary<int, InstructionValues> _valuesPerInstruction;

        public HitsInfo(IEnumerable<HitContext> contexts)
        {
            _valuesPerInstruction = HitContext.MergeDuplicates(contexts)
                .SelectMany(c => c.Hits.Keys, (context, instructionId) => new {
                    context,
                    instructionId,
                    hitCount = context.GetHitCount(instructionId)
                })
                .GroupBy(g => g.instructionId)
                .ToDictionary(g => g.Key, g => new InstructionValues
                {
                    HitCount = g.Sum(d => d.hitCount),
                    Contexts = g.Select(d => d.context).ToArray()
                });
        }

        public bool IsInstructionHit(int id)
        {
            return _valuesPerInstruction.ContainsKey(id);
        }

        public int GetInstructionHitCount(int instructionId)
        {
            if (!_valuesPerInstruction.TryGetValue(instructionId, out var values))
                return 0;

            return values.HitCount;
        }

        public IEnumerable<HitContext> GetInstructionHitContexts(int instructionId)
        {
            if (!_valuesPerInstruction.TryGetValue(instructionId, out var values))
                return Enumerable.Empty<HitContext>();

            return values.Contexts;
        }

        public static HitsInfo TryReadFromDirectory(string path)
        {
            var contexts = new List<HitContext>();

            foreach (var hitFile in Directory.GetFiles(path, "*.hits"))
            {
                using (var fileStream = File.Open(hitFile, FileMode.Open, FileAccess.Read))
                {
                    contexts.AddRange(HitContext.Deserialize(fileStream));
                }
            }

            return new HitsInfo(contexts);
        }

        class InstructionValues
        {
            public int HitCount { get; set; }
            public HitContext[] Contexts { get; set; }
        }
    }
}