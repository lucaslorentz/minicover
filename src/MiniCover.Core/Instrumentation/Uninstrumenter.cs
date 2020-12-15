using System.IO;
using System.IO.Abstractions;
using MiniCover.Core.Model;
using MiniCover.Core.Utils;

namespace MiniCover.Core.Instrumentation
{
    public class Uninstrumenter : IUninstrumenter
    {
        private readonly DepsJsonUtils _depsJsonUtils;
        private readonly IFileSystem _fileSystem;

        public Uninstrumenter(
            DepsJsonUtils depsJsonUtils,
            IFileSystem fileSystem)
        {
            _depsJsonUtils = depsJsonUtils;
            _fileSystem = fileSystem;
        }

        public void Execute(InstrumentationResult result)
        {
            foreach (var assembly in result.Assemblies)
            {
                foreach (var assemblyLocation in assembly.Locations)
                {
                    if (File.Exists(assemblyLocation.BackupFile))
                    {
                        File.Copy(assemblyLocation.BackupFile, assemblyLocation.File, true);
                        File.Delete(assemblyLocation.BackupFile);
                    }

                    if (File.Exists(assemblyLocation.BackupPdbFile))
                    {
                        File.Copy(assemblyLocation.BackupPdbFile, assemblyLocation.PdbFile, true);
                        File.Delete(assemblyLocation.BackupPdbFile);
                    }

                    var assemblyDirectory = _fileSystem.FileInfo.FromFileName(assemblyLocation.File).Directory;
                    foreach (var depsJsonFile in assemblyDirectory.GetFiles("*.deps.json"))
                    {
                        _depsJsonUtils.UnpatchDepsJson(depsJsonFile);
                    }
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
