using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MiniCover.HitServices;
using Newtonsoft.Json.Serialization;

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
            var json = File.ReadAllText(file);

            return ConvertToHits($"[{json}]");
        }

        public static Hits ConvertToHits(string json)
        {
            var notMergedHits = JsonConvert.DeserializeObject<HitTestMethod[]>(json, new JsonSerializerSettings {
                    ContractResolver = new CustomContractResolver()
                })
                .SelectMany(method => method.HitedInstructions.Select(id => new Hit(id.Key, id.Value, new[] {method}))).ToArray();
            return new Hits(notMergedHits);
        }
    }

    public class CustomContractResolver : DefaultContractResolver {

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var c = base.CreateObjectContract(objectType);
            if (!IsCustomStruct(objectType)) return c;

            IList<ConstructorInfo> list = objectType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).OrderBy(e => e.GetParameters().Length).ToList();
            var mostSpecific = list.LastOrDefault();
            if (mostSpecific != null)
            {
                c.OverrideCreator = CreateParameterizedConstructor(mostSpecific);
                foreach (var constructorParameter in CreateConstructorParameters(mostSpecific, c.Properties))
                {
                    c.CreatorParameters.Add(constructorParameter);
                }
            }

            return c;
        }

        protected virtual bool IsCustomStruct(Type objectType)
        {
            return objectType.IsValueType && !objectType.IsPrimitive && !objectType.IsEnum && !string.IsNullOrEmpty(objectType.Namespace) && !objectType.Namespace.StartsWith("System.");
        }

        private ObjectConstructor<object> CreateParameterizedConstructor(MethodBase method)
        {
            if(method == null) throw new ArgumentNullException(nameof(method));
            var c = method as ConstructorInfo;
            if (c != null)
                return a => c.Invoke(a);
            return a => method.Invoke(null, a);
        }
    }
}