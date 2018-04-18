using MiniCover.Commands.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MiniCover.Commands
{
    internal class ResetCommand : BaseCommand
    {
        private const string CommandDescription = "Reset hits count";
        private const string CommandName = "reset";

        private static readonly CoverageHitsFileOption CoverageHitsFileOption = new CoverageHitsFileOption();
        private static readonly WorkingDirectoryOption WorkingDirectoryOption = new WorkingDirectoryOption();

        public ResetCommand()
            : base(CommandName, CommandDescription, WorkingDirectoryOption, CoverageHitsFileOption)
        {
        }

        protected override Task<int> Execute()
        {
            WriteLine($"Reset coverage for directory: '{WorkingDirectoryOption.GetValue().FullName}' on pattern '{CoverageHitsFileOption.GetValue()}'");

            var hitsFiles = WorkingDirectoryOption.GetValue().GetFiles(CoverageHitsFileOption.GetValue(), SearchOption.AllDirectories);
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