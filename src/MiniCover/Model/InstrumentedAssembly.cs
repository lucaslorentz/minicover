using System.Collections.Generic;

namespace MiniCover.Model
{
    public class InstrumentedAssembly
    {
        public string File { get; set; }
        public Dictionary<string, SourceFile> Files = new Dictionary<string, SourceFile>();
        public string BackupFile { get; set; }
        public string PdbFile { get; set; }
        public string BackupPdbFile { get; set; }

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
