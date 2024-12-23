using System;
namespace MiniCover.CommandLine
{
    public class StringOption : ISingleValueOption
    {
        public StringOption(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Value { get; private set; }
        public string Name { get; }
        public string ShortName { get; set; }
        public string Description { get; }

        public void ReceiveValue(string value)
        {
            Value = value;
        }
    }
}
