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
    public class Instrumenter
    {
        private static readonly Assembly hitServicesAssembly = typeof(HitService).Assembly;

        private readonly AssemblyInstrumenter _assemblyInstrumenter;
        private readonly DepsJsonUtils _depsJsonUtils;
        private readonly IFileSystem _fileSystem;
        private readonly ILogger<Instrumenter> _logger;

        private readonly string[] _loadedAssemblyFiles = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .Select(a => a.Location)
            .ToArray();

        public Instrumenter(
            AssemblyInstrumenter assemblyInstrumenter,
            DepsJsonUtils depsJsonUtils,
            IFileSystem fileSystem,
            ILogger<Instrumenter> logger)
        {
            _assemblyInstrumenter = assemblyInstrumenter;
            _depsJsonUtils = depsJsonUtils;
            _fileSystem = fileSystem;
            _logger = logger;
        }

        public InstrumentationResult Execute(InstrumentationContext context)
        {
            context.Workdir = context.Workdir;

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
                    assemblyGroup);
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

        private void VisitAssemblyGroup(
            InstrumentationContext context,
            InstrumentationResult result,
            IEnumerable<IFileInfo> assemblyFiles)
        {
            using (_logger.BeginScope("Checking assembly files {assemblies}", assemblyFiles.Select(f => f.FullName), LogLevel.Information))
            {
                var instrumentedAssembly = _assemblyInstrumenter.InstrumentAssemblyFile(
                    context,
                    assemblyFiles.First());

                if (instrumentedAssembly == null)
                    return;

                foreach (var assemblyFile in assemblyFiles)
                {
                    if (_loadedAssemblyFiles.Contains(assemblyFile.FullName))
                    {
                        _logger.LogInformation("Skipping loaded assembly {assemblyFile}", assemblyFile.FullName);
                        continue;
                    }

                    var pdbFile = FileUtils.GetPdbFile(assemblyFile);
                    var assemblyBackupFile = FileUtils.GetBackupFile(assemblyFile);
                    var pdbBackupFile = FileUtils.GetBackupFile(pdbFile);

                    //Backup
                    _fileSystem.File.Copy(assemblyFile.FullName, assemblyBackupFile.FullName, true);
                    _fileSystem.File.Copy(pdbFile.FullName, pdbBackupFile.FullName, true);

                    //Override assembly
                    _fileSystem.File.Copy(instrumentedAssembly.TempAssemblyFile, assemblyFile.FullName, true);
                    _fileSystem.File.Copy(instrumentedAssembly.TempPdbFile, pdbFile.FullName, true);

                    //Copy instrumentation dependencies
                    var assemblyDirectory = assemblyFile.Directory;

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
