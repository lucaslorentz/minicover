using Microsoft.Extensions.CommandLineUtils;
using MiniCover.Commands.Options;
using MiniCover.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MiniCover.Commands
{
    internal class UninstrumentCommand : BaseCommandLineApplication
    {
        private readonly WorkingDirectoryOption _workingDirectoryOption = new WorkingDirectoryOption();
        private readonly IMiniCoverOption<InstrumentationResult> _coverageLoadedFileOption = new CoverageLoadedFileOption();

        public UninstrumentCommand(CommandLineApplication parentCommandLineApplication)
            : base(parentCommandLineApplication)
        {
        }

        protected override string CommandDescription => "Uninstrument assemblies";
        protected override string CommandName => "uninstrument";
        protected override IEnumerable<IMiniCoverOption> MiniCoverOptions => new IMiniCoverOption[] { _workingDirectoryOption, _coverageLoadedFileOption };

        protected override Task<int> Execution()
        {
            var instrumentationResult = _coverageLoadedFileOption.Value;
            var assemblyLocations = instrumentationResult.Assemblies.SelectMany(x => x.Locations);
            foreach (var assemblyLocation in assemblyLocations)
            {
                HandleBackupFile(assemblyLocation.BackupFile, assemblyLocation.File);
                HandleBackupFile(assemblyLocation.BackupPdbFile, assemblyLocation.PdbFile);
            }

            foreach (var extraAssembly in instrumentationResult.ExtraAssemblies)
            {
                if (File.Exists(extraAssembly))
                {
                    File.Delete(extraAssembly);
                }
            }

            return Task.FromResult(0);
        }

        private static void HandleBackupFile(string backupPath, string path)
        {
            if (!File.Exists(backupPath)) return;
            
            File.Copy(backupPath, path, true);
            File.Delete(backupPath);
        }
    }
}