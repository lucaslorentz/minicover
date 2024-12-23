namespace MiniCover.CommandLine.Options
{
    public class NoFailOption : INoValueOption, INoFailOption
    {
        public bool Value { get; private set; }
        public string Name => "--no-fail";
        public string Description => $"Do not fail this command if threshold is not met";

        public void ReceiveValue(bool value)
        {
            Value = value;
        }
    }
}