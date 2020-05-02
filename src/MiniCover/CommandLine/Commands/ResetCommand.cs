using System;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MiniCover.CommandLine;
using MiniCover.CommandLine.Options;
using MiniCover.Infrastructure;

namespace MiniCover.Commands
{
    class ResetCommand : BaseCommand
    {
        private const string _name = "reset";
        private const string _description = "Reset hits count";

        private readonly HitsDirectoryOption _hitsDirectoryOption;

        private readonly IOutput _console;

        public ResetCommand(
            IOutput console,
            VerbosityOption verbosityOption,
            WorkingDirectoryOption workingDirectoryOption,
            HitsDirectoryOption hitsDirectoryOption)
            : base(_name, _description)
        {
            _console = console;
            _hitsDirectoryOption = hitsDirectoryOption;

            Options = new IOption[]
            {
                verbosityOption,
                workingDirectoryOption,
                hitsDirectoryOption
            };
        }

        protected override Task<int> Execute()
        {
            var hitsDirectory = _hitsDirectoryOption.Value;

            _console.WriteLine($"Resetting hit directory '{hitsDirectory.FullName}'", LogLevel.Information);

            var hitsFiles = hitsDirectory.Exists
                ? hitsDirectory.GetFiles("*.hits")
                : new IFileInfo[0];

            if (!hitsFiles.Any())
            {
                _console.WriteLine("Directory is already cleared", LogLevel.Information);
                return Task.FromResult(0);
            }

            _console.WriteLine($"Found {hitsFiles.Length} files to clear", LogLevel.Information);

            var errorsCount = 0;
            foreach (var hitsFile in hitsFiles)
            {
                try
                {
                    hitsFile.Delete();
                    _console.WriteLine($"{hitsFile.FullName} - removed", LogLevel.Trace);
                }
                catch (Exception e)
                {
                    errorsCount++;
                    _console.WriteLine($"{hitsFile.FullName} - error: {e.Message}", LogLevel.Error);
                }
            }

            if (errorsCount != 0)
            {
                _console.WriteLine($"Reset operation completed with {errorsCount} errors", LogLevel.Error);
                return Task.FromResult(1);
            }

            _console.WriteLine("Reset operation completed without errors", LogLevel.Information);
            return Task.FromResult(0);
        }
    }
}