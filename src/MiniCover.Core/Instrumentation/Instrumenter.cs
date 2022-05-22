using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using MiniCover.Core.Model;
using MiniCover.Core.Utils;
using MiniCover.HitServices;

namespace MiniCover.Core.Instrumentation
{
    public class Instrumenter : IInstrumenter
    {
        private static readonly Assembly hitServicesAssembly = typeof(HitService).Assembly;

        private readonly IAssemblyInstrumenter _assemblyInstrumenter;
        private readonly DepsJsonUtils _depsJsonUtils;
        private readonly IFileSystem _fileSystem;
        private readonly ILogger<Instrumenter> _logger;

        private readonly string[] _loadedAssemblyFiles = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .Select(a => a.Location)
            .ToArray();

        public Instrumenter(
            IAssemblyInstrumenter assemblyInstrumenter,
            DepsJsonUtils depsJsonUtils,
            IFileSystem fileSystem,
            ILogger<Instrumenter> logger)
        {
            _assemblyInstrumenter = assemblyInstrumenter;
            _depsJsonUtils = depsJsonUtils;
            _fileSystem = fileSystem;
            _logger = logger;
        }

        public InstrumentationResult Instrument(IInstrumentationContext context)
        {
            var result = new InstrumentationResult
            {
                SourcePath = context.Workdir.FullName,
                HitsPath = context.HitsPath
            };

            var assemblyGroups = context.Assemblies
                .Where(ShouldInstrumentAssemblyFile)
                .GroupBy(FileUtils.GetFileHash)
                .ToArray();

            foreach (var assemblyGroup in assemblyGroups)
            {
                VisitAssemblyGroup(
                    context,
                    result,
                    assemblyGroup.ToArray());
            }

            return result;
        }

        private bool ShouldInstrumentAssemblyFile(IFileInfo assemblyFile)
        {
            if (FileUtils.IsBackupFile(assemblyFile))
                return false;

            if (assemblyFile.Name == "MiniCover.HitServices.dll")
                return false;

            if (!FileUtils.GetPdbFile(assemblyFile).Exists)
                return false;

            return true;
        }

        /// <summary>
        /// An assembly group is a set of assemblies that are identical (the same file hash). This method makes sure we only instrument the first assembly in the group, to avoid unnecessary overhead.
        /// </summary>
        private void VisitAssemblyGroup(
            IInstrumentationContext context,
            InstrumentationResult result,
            IReadOnlyCollection<IFileInfo> assemblyFiles)
        {
            using (_logger.BeginScope("Checking assembly files {assemblies}", assemblyFiles.Select(f => f.FullName)))
            {
                var instrumentedAssembly = _assemblyInstrumenter.InstrumentAssemblyFile(
                    context,
                    assemblyFiles.First());

                if (instrumentedAssembly == null)
                    return;
                
                _logger.LogTrace("Temporary assembly file: {tempAssemblyFile}", instrumentedAssembly.TempAssemblyFile);
                _logger.LogTrace("Temporary PDB file: {tempPdbFile}", instrumentedAssembly.TempPdbFile);

                foreach (var assemblyFile in assemblyFiles)
                {
                    _logger.LogDebug("Instrumenting assembly file {assemblyFile}", assemblyFile.FullName);
                    
                    if (_loadedAssemblyFiles.Contains(assemblyFile.FullName))
                    {
                        _logger.LogInformation("Skipping loaded assembly {assemblyFile}", assemblyFile.FullName);
                        continue;
                    }

                    var pdbFile = FileUtils.GetPdbFile(assemblyFile);
                    var assemblyBackupFile = FileUtils.GetBackupFile(assemblyFile);
                    var pdbBackupFile = FileUtils.GetBackupFile(pdbFile);
                    
                    _logger.LogTrace("PDB file: {pdbFileName}", pdbFile.FullName);
                    _logger.LogTrace("Assembly backup file: {assemblyBackupFileName}", assemblyBackupFile.FullName);
                    _logger.LogTrace("PDB backup file: {pdbBackupFileName}", pdbBackupFile.FullName);

                    //Backup
                    _fileSystem.File.Copy(assemblyFile.FullName, assemblyBackupFile.FullName, true);
                    _fileSystem.File.Copy(pdbFile.FullName, pdbBackupFile.FullName, true);

                    //Override assembly
                    _fileSystem.File.Copy(instrumentedAssembly.TempAssemblyFile, assemblyFile.FullName, true);
                    _fileSystem.File.Copy(instrumentedAssembly.TempPdbFile, pdbFile.FullName, true);

                    //Copy instrumentation dependencies
                    var assemblyDirectory = assemblyFile.Directory;
                    _logger.LogTrace("Assembly directory: {assemblyDirectory}", assemblyDirectory.FullName);

                    var hitServicesPath = Path.GetFileName(hitServicesAssembly.Location);
                    var newHitServicesPath = Path.Combine(assemblyDirectory.FullName, hitServicesPath);
                    _fileSystem.File.Copy(hitServicesAssembly.Location, newHitServicesPath, true);
                    result.AddExtraAssembly(newHitServicesPath);

                    instrumentedAssembly.AddLocation(
                        assemblyFile.FullName,
                        assemblyBackupFile.FullName,
                        pdbFile.FullName,
                        pdbBackupFile.FullName
                    );

                    var hitServicesAssemblyVersion = FileVersionInfo.GetVersionInfo(hitServicesAssembly.Location);
                    foreach (var depsJsonFile in assemblyDirectory.GetFiles("*.deps.json"))
                    {
                        _depsJsonUtils.PatchDepsJson(depsJsonFile, hitServicesAssemblyVersion.ProductVersion);
                    }
                }

                result.AddInstrumentedAssembly(instrumentedAssembly);

                _fileSystem.File.Delete(instrumentedAssembly.TempAssemblyFile);
                _fileSystem.File.Delete(instrumentedAssembly.TempPdbFile);
            }
        }
    }
}
