using Microsoft.Extensions.CommandLineUtils;
using MiniCover.Commands.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiniCover.Commands.Options.FileParameterizations;

namespace MiniCover.Commands
{
    internal abstract class ParameterizationCommand : BaseCommand
    {
        protected MiniCoverParameterization Parametrization;
        private readonly IMiniCoverOption<MiniCoverParameterization> _parameterizationOption;
        protected ParameterizationCommand(string commandName, string commandDescription, params IMiniCoverOption[] options) 
            : base(commandName, commandDescription, options)
        {
            _parameterizationOption = new ParameterizationOption(options.OfType<IMiniCoverParameterizationOption>().ToArray());
        }

        protected override void AddOptions(CommandLineApplication command)
        {
            base.AddOptions(command);
            _parameterizationOption.AddTo(command);
        }

        protected override void ValidateOptions()
        {
            base.ValidateOptions();
            _parameterizationOption.Validate();
            Parametrization = _parameterizationOption.GetValue();
        }
    }
    internal abstract class BaseCommand
    {
        protected readonly List<IMiniCoverOption> MiniCoverOptions;
        private const string CommandHelpOption = "-h | --help";

        private readonly string _commandDescription;
        private readonly string _commandName;

        protected BaseCommand(string commandName, string commandDescription, params IMiniCoverOption[] options)
        {
            _commandName = commandName;
            _commandDescription = commandDescription;
            MiniCoverOptions = options.SelectMany(x => x.NestedOptions()).ToList();
            MiniCoverOptions.AddRange(options);
        }

        public void AddTo(CommandLineApplication parentCommandLineApplication)
        {
            parentCommandLineApplication
                .Command(_commandName, command =>
                {
                    command.Description = _commandDescription;
                    command.HelpOption(CommandHelpOption);
                    AddOptions(command);
                    command.OnExecute(() =>
                    {
                        ValidateOptions();
                        return Execute();
                    });
                });
        }

        protected abstract Task<int> Execute();

        protected void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        protected void WriteNegativeLine(string message, Exception exception = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            if (exception != null) Console.WriteLine(exception);
            Console.ResetColor();
        }

        protected void WritePositiveLine(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        protected virtual void AddOptions(CommandLineApplication command)
        {
            foreach (var miniCoverOption in MiniCoverOptions)
            {
                if (miniCoverOption == null)
                {
                    throw new NullReferenceException("MiniCover option is null");
                }
                miniCoverOption.AddTo(command);
            }
        }

        protected virtual void ValidateOptions()
        {
            foreach (var miniCoverOption in MiniCoverOptions)
            {
                miniCoverOption.Validate();
            }
        }
    }
}