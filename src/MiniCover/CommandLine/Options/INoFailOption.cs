namespace MiniCover.CommandLine.Options
{
    public interface INoFailOption : IOption
    {
        bool Value { get; }
    }
}