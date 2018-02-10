using System.Collections.Generic;
using Newtonsoft.Json;

namespace MiniCover.Model
{
    public class InstrumentedAssembly
    {
        [JsonProperty(Order = -2)]
        public string Name { get; set; }

        [JsonProperty(Order = -2)]
        public string Hash { get; set; }
        
        public SortedDictionary<string, SourceFile> SourceFiles = new SortedDictionary<string, SourceFile>();
        public List<AssemblyLocation> Locations = new List<AssemblyLocation>();

        public void AddInstruction(string file, InstrumentedInstruction instruction)
        {
            if (!SourceFiles.ContainsKey(file))
            {
                SourceFiles[file] = new SourceFile();
            }

            SourceFiles[file].Instructions.Add(instruction);
        }

        public void AddLocation(string file, string backupFile, string pdbFile, string backupPdbFile)
        {
            var assemblyLocation = new AssemblyLocation
            {
                File = file,
                BackupFile = backupFile,
                PdbFile = pdbFile,
                BackupPdbFile = backupPdbFile
            };

            Locations.Add(assemblyLocation);
        }
    }
}
