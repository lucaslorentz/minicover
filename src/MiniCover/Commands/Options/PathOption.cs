namespace MiniCover.Commands.Options
{
    internal abstract class PathOption : MiniCoverOption<string>
    {
        private readonly string _defaultValue;

        protected PathOption(string defaultValue, string description, string optionTemplate)
            : base(description, optionTemplate)
        {
            _defaultValue = defaultValue;
        }

        protected override string GetOptionValue()
        {
            var proposalFilePath = Option.Value();
            if (string.IsNullOrWhiteSpace(proposalFilePath))
                proposalFilePath = _defaultValue;

            return proposalFilePath;
        }

        protected override bool Validation() => true;
    }
}