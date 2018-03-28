using Microsoft.Extensions.CommandLineUtils;
using MiniCover.Commands.Options.FileParameterizations;
using System;

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

    internal interface IMiniCoverParameterizationOption : IMiniCoverOption
    {
        Action<MiniCoverParameterization> SetParameter { get; }
    }
}