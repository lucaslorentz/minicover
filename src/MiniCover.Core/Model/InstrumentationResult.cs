using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MiniCover.Core.Model
{
    public class InstrumentationResult
    {
        private readonly List<InstrumentedAssembly> _assemblies;
        private readonly HashSet<string> _extraAssemblies;

        public InstrumentationResult()
        {
            _assemblies = new List<InstrumentedAssembly>();
            _extraAssemblies = new HashSet<string>();
        }

        [JsonConstructor]
        protected InstrumentationResult(
            InstrumentedAssembly[] assemblies,
            string[] extraAssemblies)
        {
            _assemblies = assemblies?.ToList() ?? new List<InstrumentedAssembly>();
            _extraAssemblies = extraAssemblies != null
                ? new HashSet<string>(extraAssemblies)
                : new HashSet<string>();
        }

        [JsonProperty(Order = -2)]
        public string SourcePath { get; set; }

        [JsonProperty(Order = -2)]
        public string HitsPath { get; set; }

        public IEnumerable<InstrumentedAssembly> Assemblies => _assemblies;
        public IEnumerable<string> ExtraAssemblies => _extraAssemblies;

        public void AddInstrumentedAssembly(InstrumentedAssembly instrumentedAssembly)
        {
            _assemblies.Add(instrumentedAssembly);
        }

        public void AddExtraAssembly(string file)
        {
            _extraAssemblies.Add(file);
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
