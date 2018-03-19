using Microsoft.Extensions.CommandLineUtils;

namespace MiniCover.Commands.Options
{
    internal interface IMiniCoverOption
    {
        void AddTo(CommandLineApplication command);

        void Validate();
    }

    internal interface IMiniCoverOption<out T> : IMiniCoverOption
    {
        T Value { get; }
    }
}