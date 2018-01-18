using MiniCover.Model;
using System.IO;

namespace MiniCover.Instrumentation
{
    public static class Uninstrumenter
    {
        public static void Execute(InstrumentationResult result)
        {
            foreach (var assembly in result.Assemblies)
            {
                if (File.Exists(assembly.Value.BackupFile))
                {
                    File.Copy(assembly.Value.BackupFile, assembly.Value.File, true);
                    File.Delete(assembly.Value.BackupFile);
                }

                if (File.Exists(assembly.Value.BackupPdbFile))
                {
                    File.Copy(assembly.Value.BackupPdbFile, assembly.Value.PdbFile, true);
                    File.Delete(assembly.Value.BackupPdbFile);
                }
            }

            foreach (var extraAssembly in result.ExtraAssemblies)
            {
                if (File.Exists(extraAssembly))
                {
                    File.Delete(extraAssembly);
                }
            }
        }
    }
}
