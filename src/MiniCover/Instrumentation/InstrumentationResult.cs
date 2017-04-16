using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniCover.Instrumentation
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

    public class InstrumentedAssembly
    {
        public string BackupFile { get; set; }
        public string File { get; set; }
    }

    public class SourceFile
    {
        public List<InstrumentedInstruction> Instructions = new List<InstrumentedInstruction>();
    }

    public class InstrumentedInstruction
    {
        public int Id { get; set; }
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public int StartColumn { get; set; }
        public int EndColumn { get; set; }
        public string Instruction { get; set; }
    }
}
