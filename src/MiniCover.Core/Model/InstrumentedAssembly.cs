using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MiniCover.Core.Model
{
    public class InstrumentedAssembly
    {
        private readonly Dictionary<string, InstrumentedMethod> _methods;
        private readonly List<AssemblyLocation> _locations;
        private readonly SortedDictionary<string, SourceFile> _sourceFiles;

        public InstrumentedAssembly(string name)
        {
            Name = name;
            _methods = new Dictionary<string, InstrumentedMethod>();
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
            _methods = methods.ToDictionary(m => m.FullName, m => m);
            _locations = locations.ToList();
            _sourceFiles = new SortedDictionary<string, SourceFile>(
                sourceFiles.ToDictionary(sf => sf.Path, sf => sf));
        }

        [JsonProperty(Order = -2)]
        public string Name { get; }

        public IEnumerable<InstrumentedMethod> Methods => _methods.Values;
        public IEnumerable<AssemblyLocation> Locations => _locations;
        public IEnumerable<SourceFile> SourceFiles => _sourceFiles.Values;

        [JsonIgnore]
        public string TempAssemblyFile { get; set; }

        [JsonIgnore]
        public string TempPdbFile { get; set; }

        public InstrumentedMethod GetOrAddMethod(string @class, string name, string fullName)
        {
            if (_methods.TryGetValue(fullName, out var existingValue))
                return existingValue;

            var method = new InstrumentedMethod
            {
                Class = @class,
                Name = name,
                FullName = fullName
            };

            _methods.Add(fullName, method);

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
