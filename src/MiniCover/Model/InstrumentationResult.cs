using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace MiniCover.Model
{
    public class InstrumentationResult
    {
        [JsonProperty(Order = -2)]
        public string SourcePath { get; set; }

        [JsonProperty(Order = -2)]
        public string HitsPath { get; set; }

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

        public SortedDictionary<string, SourceFile> GetSourceFiles()
        {
            return new SortedDictionary<string, SourceFile>(Assemblies
                .SelectMany(a => a.SourceFiles)
                .GroupBy(kv => kv.Key, kv => kv.Value)
                .ToDictionary(g => g.Key, g => SourceFile.Merge(g)));
        }
    }
}
