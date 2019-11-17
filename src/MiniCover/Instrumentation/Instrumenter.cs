using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using MiniCover.HitServices;
using MiniCover.Model;
using MiniCover.Utils;

namespace MiniCover.Instrumentation
{
    public class Instrumenter
    {
        private static readonly Assembly hitServicesAssembly = typeof(HitService).Assembly;

        private readonly ILogger<Instrumenter> _logger;
        private readonly AssemblyInstrumenter _assemblyInstrumenter;

        public Instrumenter(
            ILogger<Instrumenter> logger,
            AssemblyInstrumenter assemblyInstrumenter)
        {
            _logger = logger;
            _assemblyInstrumenter = assemblyInstrumenter;
        }

        public InstrumentationResult Execute(InstrumentationContext context)
        {
            context.Workdir = context.Workdir.AddEndingDirectorySeparator();

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

        private bool ShouldInstrumentAssemblyFile(FileInfo assemblyFile)
        {
            if (FileUtils.IsBackupFile(assemblyFile))
                return false;

            if (!FileUtils.GetPdbFile(assemblyFile).Exists)
                return false;

            return true;
        }

        private void VisitAssemblyGroup(
            InstrumentationContext context,
            InstrumentationResult result,
            IEnumerable<FileInfo> groupFiles)
        {
            var firstAssemblyFile = groupFiles.First();

            var instrumentedAssembly = _assemblyInstrumenter.InstrumentAssembly(
                context,
                firstAssemblyFile);

            if (instrumentedAssembly == null)
                return;

            foreach (var assemblyFile in groupFiles)
            {
                var pdbFile = FileUtils.GetPdbFile(assemblyFile);
                var assemblyBackupFile = FileUtils.GetBackupFile(assemblyFile);
                var pdbBackupFile = FileUtils.GetBackupFile(pdbFile);

                //Backup
                File.Copy(assemblyFile.FullName, assemblyBackupFile.FullName, true);
                File.Copy(pdbFile.FullName, pdbBackupFile.FullName, true);

                //Override assembly
                File.Copy(instrumentedAssembly.TempAssemblyFile, assemblyFile.FullName, true);
                File.Copy(instrumentedAssembly.TempPdbFile, pdbFile.FullName, true);

                //Copy instrumentation dependencies
                var assemblyDirectory = assemblyFile.Directory;

                var hitServicesPath = Path.GetFileName(hitServicesAssembly.Location);
                var newHitServicesPath = Path.Combine(assemblyDirectory.FullName, hitServicesPath);
                if (!File.Exists(newHitServicesPath))
                {
                    File.Copy(hitServicesAssembly.Location, newHitServicesPath, true);
                    result.AddExtraAssembly(newHitServicesPath);
                }

                instrumentedAssembly.AddLocation(
                    assemblyFile.FullName,
                    assemblyBackupFile.FullName,
                    pdbFile.FullName,
                    pdbBackupFile.FullName
                );

                var hitServicesAssemblyVersion = FileVersionInfo.GetVersionInfo(hitServicesAssembly.Location);
                foreach (var depsJsonFile in assemblyDirectory.GetFiles("*.deps.json"))
                {
                    DepsJsonUtils.PatchDepsJson(depsJsonFile, hitServicesAssemblyVersion.ProductVersion);
                }
            }

            result.AddInstrumentedAssembly(instrumentedAssembly);

            File.Delete(instrumentedAssembly.TempAssemblyFile);
            File.Delete(instrumentedAssembly.TempPdbFile);
        }
    }
}
