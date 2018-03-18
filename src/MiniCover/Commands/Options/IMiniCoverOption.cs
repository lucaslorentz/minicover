using Microsoft.Extensions.CommandLineUtils;

namespace MiniCover.Commands.Options
{
    internal interface IMiniCoverOption
    {
        void Validate();

        void Initialize(CommandLineApplication commandContext);
    }

    internal interface IMiniCoverOption<out T> : IMiniCoverOption
    {
        T Value { get; }
    }
}