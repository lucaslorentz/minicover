using Microsoft.Extensions.CommandLineUtils;

namespace MiniCover.Commands.Options
{
    internal interface IMiniCoverOption
    {
        string OptionTemplate { get; }
        string Description { get; }
        CommandOptionType Type { get; }

        void Invalidate();

        void Initialize(CommandLineApplication commandContext);
    }

    internal interface IMiniCoverOption<out T> : IMiniCoverOption
    {
        T Value { get; }
    }
}