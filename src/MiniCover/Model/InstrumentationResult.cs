using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MiniCover.Model
{
    public class InstrumentationResult
    {
        [JsonProperty(Order = -2)]
        public string SourcePath { get; set; }

        [JsonProperty(Order = -2)]
        public string HitsFile { get; set; }

        public HashSet<string> ExtraAssemblies = new HashSet<string>();

        public List<InstrumentedAssembly> Assemblies = new List<InstrumentedAssembly>();

        public void AddInstrumentedAssembly(InstrumentedAssembly instrumentedAssembly)
        {
            Assemblies.Add(instrumentedAssembly);
        }

        public void AddExtraAssembly(string file)
        {
            ExtraAssemblies.Add(file);
        }

        public Dictionary<string, SourceFile> GetSourceFiles()
        {
            return Assemblies
                .SelectMany(a => a.SourceFiles)
                .GroupBy(kv => kv.Key, kv => kv.Value)
                .ToDictionary(g => g.Key, g => SourceFile.Merge(g));
        }
    }
}
