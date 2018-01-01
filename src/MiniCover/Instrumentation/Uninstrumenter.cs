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
                if (File.Exists(assembly.BackupFile))
                {
                    File.Copy(assembly.BackupFile, assembly.File, true);
                    File.Delete(assembly.BackupFile);
                }

                if (File.Exists(assembly.BackupPdbFile))
                {
                    File.Copy(assembly.BackupPdbFile, assembly.PdbFile, true);
                    File.Delete(assembly.BackupPdbFile);
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
