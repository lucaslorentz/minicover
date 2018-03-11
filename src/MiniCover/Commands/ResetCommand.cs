using Microsoft.Extensions.CommandLineUtils;
using MiniCover.Commands.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MiniCover.Commands
{
    internal class ResetCommand : BaseCommandLineApplication
    {
        private readonly CoverageHitsFileOption _coverageHitsFileOption = new CoverageHitsFileOption();
        private readonly SolutionDirOption _solutionDirOption = new SolutionDirOption();

        public ResetCommand(CommandLineApplication parentCommandLineApplication)
            : base(parentCommandLineApplication)
        {
        }

        protected override string CommandDescription => "Reset hits count";
        protected override string CommandName => "reset";
        protected override IEnumerable<IMiniCoverOption> MiniCoverOptions => new IMiniCoverOption[] { _solutionDirOption, _coverageHitsFileOption };

        protected override Task<int> Execution()
        {
            var errorsCount = 0;
            Console.WriteLine($"Reset coverage for solution directory: '{_solutionDirOption.Value.FullName}' on pattern '{_coverageHitsFileOption.Value}'");
            var hitsFiles = _solutionDirOption.Value.GetFiles(_coverageHitsFileOption.Value, SearchOption.AllDirectories);

            if (!hitsFiles.Any())
            {
                PositiveLine("Solution is already cleared");
                return Task.FromResult(0);
            }

            Line($"Found {hitsFiles.Length} files to clear");
            foreach (var hitsFile in hitsFiles)
            {
                if (!File.Exists(hitsFile.FullName)) continue;
                try
                {
                    hitsFile.Delete();
                    Line($"{hitsFile.FullName} - removed");
                }
                catch (Exception e)
                {
                    errorsCount++;
                    BadLine($"{hitsFile.FullName} - skipped (error: {e.Message})");
                }
            }

            if (errorsCount != 0)
            {
                BadLine($"Reset operation completed with {errorsCount} errors");
            }
            else
            {
                PositiveLine("Reset operation completed without errors");
                return Task.FromResult(0);
            }

            return Task.FromResult(0);
        }
    }
}