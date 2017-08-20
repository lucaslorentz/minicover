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
        public List<InstrumentedAssembly> Assemblies = new List<InstrumentedAssembly>();
        public Dictionary<string, SourceFile> Files = new Dictionary<string, SourceFile>();

        public void AddInstrumentedAssembly(string originalFile, string instrumentedFile)
        {
            Assemblies.Add(new InstrumentedAssembly
            {
                BackupFile = originalFile,
                File = instrumentedFile
            });
        }

        public void AddExtraAssembly(string file)
        {
            if (!ExtraAssemblies.Contains(file))
                ExtraAssemblies.Add(file);
        }

        public void AddInstruction(string file, InstrumentedInstruction instruction)
        {
            if (!Files.ContainsKey(file))
            {
                Files[file] = new SourceFile();
            }

            Files[file].Instructions.Add(instruction);
        }
    }
}
