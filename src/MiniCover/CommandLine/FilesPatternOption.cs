using System.Collections.Generic;

namespace MiniCover.CommandLine.Options
{
    abstract class FilesPatternOption : MultiValueOption<IList<string>>
    {
        protected FilesPatternOption(string template, string description)
            : base(template, description)
        {
        }

        protected override IList<string> PrepareValue(IList<string> values)
        {
            if (values.Count == 0)
                return GetDefaultValue();

            return values;
        }

        protected abstract IList<string> GetDefaultValue();
    }
}