using Microsoft.Extensions.CommandLineUtils;
using MiniCover.Commands.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniCover.Commands
{
    internal abstract class BaseCommandLineApplication : CommandLineApplication
    {
        private const string CommandHelpOption = "-h | --help";

        protected BaseCommandLineApplication(CommandLineApplication parentCommandLineApplication)
        {
            Initialize();
            Parent = parentCommandLineApplication;
            HelpOption(CommandHelpOption);
            InitializeOptions();
            InitializeExecution();
        }

        protected abstract string CommandDescription { get; }
        protected abstract string CommandName { get; }
        protected abstract IEnumerable<IMiniCoverOption> MiniCoverOptions { get; }

        protected abstract Task<int> Execution();

        private void Initialize()
        {
            Name = CommandName;
            Description = CommandDescription;
        }

        private void InitializeOptions()
        {
            foreach (var miniCoverOption in MiniCoverOptions)
            {
                if (miniCoverOption == null)
                {
                    throw new ArgumentNullException(nameof(miniCoverOption), "MiniCover option is null");
                }
                miniCoverOption.Initialize(this);
            }
        }

        private void InvalidateOptions()
        {
            foreach (var miniCoverOption in MiniCoverOptions)
            {
                miniCoverOption.Invalidate();
            }
        }

        private void InitializeExecution()
        {
            OnExecute(() =>
            {
                InvalidateOptions();
                return Execution();
            });
        }

        protected void PositiveLine(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        protected void BadLine(string message, Exception exception = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            if (exception != null) Console.WriteLine(exception);
            Console.ResetColor();
        }

        protected void Line(string message)
        {
            Console.WriteLine(message);
        }
    }
}