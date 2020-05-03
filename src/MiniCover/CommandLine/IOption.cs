using System.Collections.Generic;

namespace MiniCover.CommandLine
{
    interface IOption
    {
        string Template { get; }
        string Description { get; }
    }

    interface ISingleValueOption : IOption
    {
        void ReceiveValue(string value);
    }

    interface IMultiValueOption : IOption
    {
        void ReceiveValue(IList<string> values);
    }
}