using System.Collections.Generic;
using Newtonsoft.Json;

namespace MiniCover.Model
{
    public class InstrumentedAssembly
    {
        [JsonConstructor]
        public InstrumentedAssembly(string name)
        {
            Name = name;
        }

        [JsonProperty(Order = -2)]
        public string Name { get; }

        public HashSet<InstrumentedMethod> Methods = new HashSet<InstrumentedMethod>();
        public List<AssemblyLocation> Locations = new List<AssemblyLocation>();
        public SortedDictionary<string, SourceFile> SourceFiles = new SortedDictionary<string, SourceFile>();

        [JsonIgnore]
        public string TempAssemblyFile { get; set; }

        [JsonIgnore]
        public string TempPdbFile { get; set; }

        public InstrumentedMethod AddMethod(InstrumentedMethod method)
        {
            if (Methods.TryGetValue(method, out var existingValue))
                return existingValue;

            Methods.Add(method);
            return method;
        }

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
