using Microsoft.Extensions.CommandLineUtils;

namespace MiniCover.CommandLine
{
    interface IOption
    {
        void AddTo(CommandLineApplication command);

        void Prepare();
    }

    interface IOption<out T> : IOption
    {
        T Value { get; }
    }
}