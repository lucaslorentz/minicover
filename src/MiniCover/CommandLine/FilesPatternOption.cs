using System.Collections.Generic;
using System.Linq;

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

            return values.Where(v => !string.IsNullOrEmpty(v)).ToArray();
        }

        protected abstract IList<string> GetDefaultValue();
    }
}