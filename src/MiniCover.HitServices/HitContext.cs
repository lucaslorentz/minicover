using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MiniCover.HitServices
{
    public class HitContext
    {
        private static readonly AsyncLocal<HitContext> _currentAsyncLocal = new AsyncLocal<HitContext>();

        public static HitContext Current
        {
            get => _currentAsyncLocal.Value;
            set => _currentAsyncLocal.Value = value;
        }

        private readonly object _lock = new object();

        public HitContext(
            string assemblyName,
            string className,
            string methodName,
            IDictionary<int, int> hits = null)
        {
            AssemblyName = assemblyName;
            ClassName = className;
            MethodName = methodName;
            Hits = hits ?? new Dictionary<int, int>();
        }

        public string AssemblyName { get; }
        public string ClassName { get; }
        public string MethodName { get; }
        public IDictionary<int, int> Hits { get; }

        public void RecordHit(int id)
        {
            lock (_lock)
            {
                if (Hits.TryGetValue(id, out var count))
                {
                    Hits[id] = count + 1;
                }
                else
                {
                    Hits[id] = 1;
                }
            }
        }

        public int GetHitCount(int id)
        {
            if (!Hits.TryGetValue(id, out var count))
                return 0;

            return count;
        }

        public static IEnumerable<HitContext> MergeDuplicates(IEnumerable<HitContext> source)
        {
            return source
                .GroupBy(h => new { h.AssemblyName, h.ClassName, h.MethodName })
                .Select(g => new HitContext(
                    g.Key.AssemblyName,
                    g.Key.ClassName,
                    g.Key.MethodName,
                    g.SelectMany(m => m.Hits)
                        .GroupBy(kv => kv.Key)
                        .ToDictionary(g2 => g2.Key, g2 => g2.Sum(kv => kv.Value))
                )).ToArray();
        }

        public void Serialize(Stream stream)
        {
            lock (_lock)
            {
                using (var binaryWriter = new BinaryWriter(stream, Encoding.UTF8, true))
                {
                    binaryWriter.Write(ClassName);
                    binaryWriter.Write(MethodName);
                    binaryWriter.Write(AssemblyName);
                    binaryWriter.Write(Hits.Count);
                    foreach (var hitedInstruction in Hits)
                    {
                        binaryWriter.Write(hitedInstruction.Key);
                        binaryWriter.Write(hitedInstruction.Value);
                    }
                }
            }
        }

        public static IEnumerable<HitContext> Deserialize(Stream stream)
        {
            var result = new List<HitContext>();
            using (var binaryReader = new BinaryReader(stream, Encoding.UTF8))
            {
                while (stream.Position < stream.Length)
                {
                    result.Add(Read(binaryReader));
                }
            }

            return result;
        }

        public static HitContext Read(BinaryReader binaryReader)
        {
            var className = binaryReader.ReadString();
            var methodName = binaryReader.ReadString();
            var assemblyName = binaryReader.ReadString();
            var hitsCount = binaryReader.ReadInt32();
            var hits = new Dictionary<int, int>();
            for (int i = 0; i < hitsCount; i++)
            {
                var instructionId = binaryReader.ReadInt32();
                var instructionHit = binaryReader.ReadInt32();
                hits.Add(instructionId, instructionHit);
            }
            return new HitContext(assemblyName, className, methodName, hits);
        }
    }
}