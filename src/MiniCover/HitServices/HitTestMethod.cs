using System.Collections.Concurrent;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MiniCover
{
    public class HitTestMethod
    {
        private readonly ConcurrentDictionary<int, int> hitedInstructions = new ConcurrentDictionary<int, int>();
        private readonly IDictionary<int, int> deserializeHitedInstructions;
        internal HitTestMethod(MethodBase testMethod)
        {
            AssemblyName = testMethod.DeclaringType.Assembly.FullName;
            ClassName = testMethod.DeclaringType.Name;
            MethodName = testMethod.Name;
            AssemblyLocation = testMethod.DeclaringType.Assembly.Location;
        }

        [JsonConstructor]

        internal HitTestMethod(string assemblyName,
            string className,
            string methodName,
            string assemblyLocation,
            int counter, IDictionary<int, int> hitedInstructions)
        {
            AssemblyName = assemblyName;
            ClassName = className;
            MethodName = methodName;
            AssemblyLocation = assemblyLocation;
            Counter = counter;
            deserializeHitedInstructions = hitedInstructions;
        }

        public string AssemblyName { get; }
        public string ClassName { get; }
        public string MethodName { get; }
        public string AssemblyLocation { get; }
        public int Counter { get; private set; }
        public IDictionary<int, int> HitedInstructions => deserializeHitedInstructions ?? hitedInstructions;

        public void Hited()
        {
            Counter++;
        }

        public void HasHit(int id)
        {
            Counter++;
            this.hitedInstructions.AddOrUpdate(id, i => 1, (i, i1) => i1 + 1);
        }

        public static HitTestMethod Convert(HitTestMethod arg, int instructionId)
        {
            return new HitTestMethod(arg.AssemblyName, arg.ClassName, arg.MethodName, arg.AssemblyLocation, arg.HitedInstructions[instructionId], new Dictionary<int, int>());
        }

        public bool HasBeenHitBy(int instructionId)
        {
            return this.HitedInstructions.ContainsKey(instructionId);
        }
        
        public static IEnumerable<HitTestMethod> MergeDuplicates(IEnumerable<HitTestMethod> source, int instructionId)
        {
            return source
                .GroupBy(h => new { h.AssemblyName, h.ClassName, h.MethodName, h.AssemblyLocation })
                .Select(g => new HitTestMethod(
                    g.Key.AssemblyName,
                    g.Key.ClassName,
                    g.Key.MethodName,
                    g.Key.AssemblyLocation,
                    g.Sum(h => h.HitedInstructions[instructionId]),
                    new Dictionary<int, int>()
                )).ToArray();
        }
    }
}