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
        private const string CoverageFileTemplate = "*.coverage.hits";
        private readonly SolutionDirOption _solutionDirOption = new SolutionDirOption();

        public ResetCommand(CommandLineApplication parentCommandLineApplication)
            : base(parentCommandLineApplication)
        {
        }

        protected override string CommandDescription => "Reset hits count";
        protected override string CommandName => "reset";
        protected override IEnumerable<IMiniCoverOption> MiniCoverOptions => new[] { _solutionDirOption };

        protected override Task<int> Execution()
        {
            var errorDictionary = new Dictionary<string, Exception>();
            Console.WriteLine($"Reset coverage for solution directory: '{_solutionDirOption.Value.FullName}'");
            var hitsFiles = _solutionDirOption.Value.GetFiles(CoverageFileTemplate, SearchOption.AllDirectories);

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
                }
                catch (Exception e)
                {
                    errorDictionary.Add($"File: '{hitsFile.FullName}' cannot be removed", e);
                }
            }

            if (errorDictionary.Any())
            {
                BadLine("Files with Error");
            }
            else
            {
                PositiveLine("Reset operation Completed without errors");
                return Task.FromResult(0);
            }

            foreach (var errorEntry in errorDictionary)
            {
                BadLine(errorEntry.Key, errorEntry.Value);
            }

            return Task.FromResult(0);
        }
    }
}