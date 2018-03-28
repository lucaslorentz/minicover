using Microsoft.Extensions.CommandLineUtils;

namespace MiniCover.Commands.Options
{
    public interface IMiniCoverOption
    {
        void AddTo(CommandLineApplication command);

        IMiniCoverOption[] NestedOptions();

        void Validate();
    }

    internal interface IMiniCoverOption<out T> : IMiniCoverOption
    {
        T GetValue();
    }

    internal interface IMiniCoverParameterizationOption<out T> : IMiniCoverOption<T>
}