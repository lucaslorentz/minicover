using Microsoft.Extensions.CommandLineUtils;
using MiniCover.Commands.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniCover.Commands
{
    internal abstract class BaseCommand
    {
        private const string CommandHelpOption = "-h | --help";

        public void AddTo(CommandLineApplication parentCommandLineApplication)
        {
            parentCommandLineApplication
                .Command(CommandName, command =>
                {
                    command.Description = CommandDescription;
                    command.HelpOption(CommandHelpOption);
                    AddOptions(command);
                    command.OnExecute(() =>
                    {
                        ValidateOptions();
                        return Execute();
                    });
                });
        }

        protected abstract string CommandName { get; }
        protected abstract string CommandDescription { get; }
        protected abstract IEnumerable<IMiniCoverOption> MiniCoverOptions { get; }

        protected abstract Task<int> Execute();

        private void AddOptions(CommandLineApplication command)
        {
            foreach (var miniCoverOption in MiniCoverOptions)
            {
                if (miniCoverOption == null)
                {
                    throw new ArgumentNullException(nameof(miniCoverOption), "MiniCover option is null");
                }
                miniCoverOption.AddTo(command);
            }
        }

        private void ValidateOptions()
        {
            foreach (var miniCoverOption in MiniCoverOptions)
            {
                miniCoverOption.Validate();
            }
        }

        protected void WritePositiveLine(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        protected void WriteNegativeLine(string message, Exception exception = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            if (exception != null) Console.WriteLine(exception);
            Console.ResetColor();
        }

        protected void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}