
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MiniCover.HitServices;

namespace MiniCover
{
    [Serializable]
    public sealed class Hit
    {
        private HashSet<TestMethodInfo> testMethodInfos = new HashSet<TestMethodInfo>();
        public int InstructionId { get; }
        public int Counter { get; private set; }

        public IEnumerable<TestMethodInfo> TestMethods => testMethodInfos.ToArray();

        internal Hit(int instructionId)
        {
            InstructionId = instructionId;
        }

        public static Hit Build(int instructionId, int counter, IEnumerable<TestMethodInfo> methods)
        {
            return new Hit(instructionId){ Counter = counter, testMethodInfos = methods.ToHashSet()};
        }

        public static Hit Merge(IEnumerable<Hit> items)
        {
            var hitsToMerge = items.ToArray();

            if (hitsToMerge.Length <= 1)
                return hitsToMerge.FirstOrDefault();

            return new Hit(hitsToMerge.First().InstructionId)
            {
                Counter = hitsToMerge.Sum(hi => hi.Counter),
                testMethodInfos = hitsToMerge.SelectMany(hi => hi.testMethodInfos).GroupBy(hi => hi).Select(TestMethodInfo.Merge).ToHashSet()
            };
        }
        public void HitedBy(TestMethodInfo testMethod)
        {
            Counter++;

            var existing = testMethodInfos.SingleOrDefault(a => a.Equals(testMethod));
            if (existing == null)
            {
                testMethodInfos.Add(testMethod.Clone());
                
            }
            else
            {
                existing.HasCall();
            }
        }
    } 
}