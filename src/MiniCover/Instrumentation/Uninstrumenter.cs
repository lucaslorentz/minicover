using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
