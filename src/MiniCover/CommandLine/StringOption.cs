using System;
namespace MiniCover.CommandLine
{
    public class StringOption : ISingleValueOption
    {
        public StringOption(string template, string description)
        {
            Template = template;
            Description = description;
        }

        public string Value { get; private set; }
        public string Template { get; }
        public string Description { get; }

        public void ReceiveValue(string value)
        {
            Value = value;
        }
    }
}
