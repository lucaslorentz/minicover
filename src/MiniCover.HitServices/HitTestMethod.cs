using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MiniCover.HitServices
{
    public class HitTestMethod
    {
        private readonly object _lock = new object();

        internal static HitTestMethod From(MethodBase testMethod)
        {
            return new HitTestMethod(testMethod.DeclaringType.Assembly.FullName,
                testMethod.DeclaringType.FullName, testMethod.Name, testMethod.DeclaringType.Assembly.Location, 0,
                new Dictionary<int, int>());
        }

        public HitTestMethod(string assemblyName,
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
            HitedInstructions = hitedInstructions;
        }

        public string AssemblyName { get; }
        public string ClassName { get; }
        public string MethodName { get; }
        public string AssemblyLocation { get; }
        public int Counter { get; private set; }
        public IDictionary<int, int> HitedInstructions { get; }

        public void HasHit(int id)
        {
            lock (_lock)
            {
                Counter++;
                if (this.HitedInstructions.TryGetValue(id, out var count))
                {
                    this.HitedInstructions[id] = count + 1;
                }
                else
                {
                    this.HitedInstructions[id] = 1;
                }
            }
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

        public void Serialize(Stream stream)
        {
            using (var binaryWriter = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                binaryWriter.Write(ClassName);
                binaryWriter.Write(MethodName);
                binaryWriter.Write(AssemblyName);
                binaryWriter.Write(AssemblyLocation);
                binaryWriter.Write(Counter);
                binaryWriter.Write(HitedInstructions.Count);
                foreach (var hitedInstruction in HitedInstructions)
                {
                    binaryWriter.Write(hitedInstruction.Key);
                    binaryWriter.Write(hitedInstruction.Value);
                }
            }
        }

        public static IEnumerable<HitTestMethod> Deserialize(Stream stream)
        {
            var result = new List<HitTestMethod>();
            using (var binaryReader = new BinaryReader(stream))
            {
                while (stream.Position < stream.Length)
                {
                   result.Add(Read(binaryReader));
                }
            }

            return result;
        }

        public static HitTestMethod Read(BinaryReader binaryReader)
        {
            var className = binaryReader.ReadString();
            var methodName = binaryReader.ReadString();
            var assemblyName = binaryReader.ReadString();
            var assemblyLocation = binaryReader.ReadString();
            var counter = binaryReader.ReadInt32();
            var hitedCount = binaryReader.ReadInt32();
            var hited = new Dictionary<int, int>();
            for (int i = 0; i < hitedCount; i++)
            {
                var instructionId = binaryReader.ReadInt32();
                var instructionHit = binaryReader.ReadInt32();
                hited.Add(instructionId, instructionHit);
            }
            return new HitTestMethod(assemblyName, className, methodName, assemblyLocation, counter, hited);
        }
    }
}