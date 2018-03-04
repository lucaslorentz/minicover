﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace MiniCover
{
    public class Hits : IEnumerable<Hit>
    {
        private readonly Dictionary<int, Hit> hits;

        internal Hits(IEnumerable<Hit> hits)
        {
            this.hits = hits
                .GroupBy(hm => hm.InstructionId)
                .ToDictionary(g => g.Key, Hit.Merge);
        }

        public Hits(): this(Enumerable.Empty<Hit>())
        {
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

        public IEnumerable<TestMethodInfo> GetInstructionTestMethods(int instructionId)
        {
            if (!hits.TryGetValue(instructionId, out var hit))
                return Enumerable.Empty<TestMethodInfo>();

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
                yield break;

            var binaryFormatter = new BinaryFormatter();

            using (var stream = File.Open(file, FileMode.Open, FileAccess.Read))
            {
                while (stream.Position < stream.Length)
                {
                    yield return binaryFormatter.Deserialize(stream) as Hit;
                }
            }
        }

        public void Hited(int id)
        {
            if (!this.hits.ContainsKey(id))
            {
                this.hits.Add(id, new Hit(id));
            }

            this.hits[id].HitedBy(TestMethodInfo.GetCurrentTestMethodInfo());
        }

        public IEnumerator<Hit> GetEnumerator()
        {
            return this.hits.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}