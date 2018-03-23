namespace MiniCover.Commands.Options
{
    internal abstract class PathOption : MiniCoverOption<string>
    {
        protected abstract string DefaultValue { get; }

        protected override string GetOptionValue()
        {
            var proposalFilePath = Option.Value();
            if (string.IsNullOrWhiteSpace(proposalFilePath))
                proposalFilePath = DefaultValue;

            return proposalFilePath;
        }
    }
}