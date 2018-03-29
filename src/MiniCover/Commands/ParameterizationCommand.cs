using System.Linq;
using Microsoft.Extensions.CommandLineUtils;
using MiniCover.Commands.Options;
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
}