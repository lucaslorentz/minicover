using MiniCover.Commands.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MiniCover.Commands
{
    internal class ResetCommand : BaseCommand
    {
        private readonly CoverageHitsFileOption _coverageHitsFileOption = new CoverageHitsFileOption();
        private readonly WorkingDirectoryOption _workingDirectoryOption = new WorkingDirectoryOption();

        protected override string CommandName => "reset";
        protected override string CommandDescription => "Reset hits count";
        protected override IEnumerable<IMiniCoverOption> MiniCoverOptions => new IMiniCoverOption[] {
            _workingDirectoryOption,
            _coverageHitsFileOption
        };

        protected override Task<int> Execute()
        {
            WriteLine($"Reset coverage for directory: '{_workingDirectoryOption.Value.FullName}' on pattern '{_coverageHitsFileOption.Value}'");

            var hitsFiles = _workingDirectoryOption.Value.GetFiles(_coverageHitsFileOption.Value, SearchOption.AllDirectories);
            if (!hitsFiles.Any())
            {
                WritePositiveLine("Directory is already cleared");
                return Task.FromResult(0);
            }

            WriteLine($"Found {hitsFiles.Length} files to clear");

            var errorsCount = 0;
            foreach (var hitsFile in hitsFiles)
            {
                try
                {
                    hitsFile.Delete();
                    WriteLine($"{hitsFile.FullName} - removed");
                }
                catch (Exception e)
                {
                    errorsCount++;
                    WriteNegativeLine($"{hitsFile.FullName} - error: {e.Message}");
                }
            }

            if (errorsCount != 0)
            {
                WriteNegativeLine($"Reset operation completed with {errorsCount} errors");
                return Task.FromResult(1);
            }

            WritePositiveLine("Reset operation completed without errors");
            return Task.FromResult(0);
        }
    }
}