using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

namespace MiniCover.CommandLine
{
    abstract class BaseCommand
    {
        private const string CommandHelpOption = "-h | --help";

        private readonly string _commandDescription;
        private readonly string _commandName;

        protected BaseCommand(
            string commandName,
            string commandDescription)
        {
            _commandName = commandName;
            _commandDescription = commandDescription;
        }

        protected virtual IOption[] Options { get; set; } = new IOption[0];

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
                        PrepareOptions();
                        return Execute();
                    });
                });
        }

        protected abstract Task<int> Execute();

        private void AddOptions(CommandLineApplication command)
        {
            foreach (var option in Options)
            {
                option.AddTo(command);
            }
        }

        private void PrepareOptions()
        {
            foreach (var option in Options)
            {
                option.Prepare();
            }
        }
    }
}