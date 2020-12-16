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
                    if (_fileSystem.File.Exists(assemblyLocation.BackupFile))
                    {
                        _fileSystem.File.Copy(assemblyLocation.BackupFile, assemblyLocation.File, true);
                        _fileSystem.File.Delete(assemblyLocation.BackupFile);
                    }

                    if (_fileSystem.File.Exists(assemblyLocation.BackupPdbFile))
                    {
                        _fileSystem.File.Copy(assemblyLocation.BackupPdbFile, assemblyLocation.PdbFile, true);
                        _fileSystem.File.Delete(assemblyLocation.BackupPdbFile);
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
                if (_fileSystem.File.Exists(extraAssembly))
                {
                    _fileSystem.File.Delete(extraAssembly);
                }
            }
        }
    }
}
