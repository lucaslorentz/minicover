using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

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

        public SourceFile[] GetSourceFiles()
        {
            return Assemblies
                .SelectMany(a => a.SourceFiles)
                .GroupBy(sf => sf.Path)
                .Select(g => SourceFile.Merge(g))
                .ToArray();
        }
    }
}
