using Newtonsoft.Json;
using System.Collections.Generic;

namespace MiniCover.Model
{
    public class InstrumentationResult
    {
        [JsonProperty(Order = -2)]
        public string SourcePath { get; set; }

        [JsonProperty(Order = -2)]
        public string HitsFile { get; set; }

        public List<string> ExtraAssemblies = new List<string>();
        public Dictionary<string, InstrumentedAssembly> Assemblies = new Dictionary<string, InstrumentedAssembly>();

        public InstrumentedAssembly AddInstrumentedAssembly(string name, string backupFile, string file, string backupPdbFile, string pdbFile)
        {
            if (Assemblies.ContainsKey(name))
            {
                return Assemblies[name];
            }

            var instrumentedAssembly = new InstrumentedAssembly
            {
                BackupFile = backupFile,
                File = file,
                BackupPdbFile = backupPdbFile,
                PdbFile = pdbFile
            };

            Assemblies.Add(name, instrumentedAssembly);

            return instrumentedAssembly;
        }

        public void AddExtraAssembly(string file)
        {
            if (!ExtraAssemblies.Contains(file))
                ExtraAssemblies.Add(file);
        }
    }
}
