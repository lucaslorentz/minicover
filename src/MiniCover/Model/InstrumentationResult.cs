using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MiniCover.Model
{
    public class InstrumentationResult
    {
        private Dictionary<string, InstrumentedAssembly> _assemblies;

        public InstrumentationResult()
        {
            _assemblies = new Dictionary<string, InstrumentedAssembly>();
        }

        [JsonConstructor]
        public InstrumentationResult(IEnumerable<InstrumentedAssembly> assemblies)
        {
            _assemblies = assemblies.ToDictionary(a => a.Hash);
        }

        [JsonProperty(Order = -2)]
        public string SourcePath { get; set; }

        [JsonProperty(Order = -2)]
        public string HitsFile { get; set; }

        public HashSet<string> ExtraAssemblies = new HashSet<string>();

        public IEnumerable<InstrumentedAssembly> Assemblies => _assemblies.Values;

        public InstrumentedAssembly GetInstrumentedAssembly(string hash)
        {
            if (!_assemblies.TryGetValue(hash, out var instrumentedAssembly))
                return null;

            return instrumentedAssembly;
        }

        public InstrumentedAssembly AddInstrumentedAssembly(string hash, string name)
        {
            var instrumentedAssembly = new InstrumentedAssembly
            {
                Hash = hash,
                Name = name
            };

            _assemblies.Add(hash, instrumentedAssembly);

            return instrumentedAssembly;
        }

        public void AddExtraAssembly(string file)
        {
            ExtraAssemblies.Add(file);
        }
    }
}
