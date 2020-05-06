using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MiniCover.Model
{
    public class InstrumentedAssembly
    {
        private readonly HashSet<InstrumentedMethod> _methods;
        private readonly List<AssemblyLocation> _locations;
        private readonly SortedDictionary<string, SourceFile> _sourceFiles;

        public InstrumentedAssembly(string name)
        {
            Name = name;
            _methods = new HashSet<InstrumentedMethod>();
            _locations = new List<AssemblyLocation>();
            _sourceFiles = new SortedDictionary<string, SourceFile>();
        }

        [JsonConstructor]
        protected InstrumentedAssembly(
            string name,
            InstrumentedMethod[] methods,
            AssemblyLocation[] locations,
            SourceFile[] sourceFiles)
        {
            Name = name;
            _methods = methods.ToHashSet();
            _locations = locations.ToList();
            _sourceFiles = new SortedDictionary<string, SourceFile>(
                sourceFiles.ToDictionary(sf => sf.Path, sf => sf));
        }

        [JsonProperty(Order = -2)]
        public string Name { get; }

        public IEnumerable<InstrumentedMethod> Methods => _methods;
        public IEnumerable<AssemblyLocation> Locations => _locations;
        public IEnumerable<SourceFile> SourceFiles => _sourceFiles.Values;

        [JsonIgnore]
        public string TempAssemblyFile { get; set; }

        [JsonIgnore]
        public string TempPdbFile { get; set; }

        public InstrumentedMethod AddMethod(InstrumentedMethod method)
        {
            if (_methods.TryGetValue(method, out var existingValue))
                return existingValue;

            _methods.Add(method);
            return method;
        }

        public void AddSequence(string file, InstrumentedSequence instruction)
        {
            if (!_sourceFiles.ContainsKey(file))
            {
                _sourceFiles[file] = new SourceFile(file);
            }

            _sourceFiles[file].Sequences.Add(instruction);
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

            _locations.Add(assemblyLocation);
        }
    }
}
