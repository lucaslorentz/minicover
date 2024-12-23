using System.Collections.Generic;

namespace MiniCover.CommandLine
{
    public interface IOption
    {
        string Name { get; }
        string ShortName => null;
        string Description { get; }
    }

    public interface INoValueOption : IOption
    {
        void ReceiveValue(bool value);
    }

    public interface ISingleValueOption : IOption
    {
        void ReceiveValue(string value);
    }

    public interface IMultiValueOption : IOption
    {
        void ReceiveValue(IList<string> values);
    }
}