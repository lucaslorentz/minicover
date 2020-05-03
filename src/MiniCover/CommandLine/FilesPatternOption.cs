using System.Collections.Generic;
using System.Linq;

namespace MiniCover.CommandLine.Options
{
    public abstract class FilesPatternOption : IMultiValueOption
    {
        public IList<string> Value { get; private set; }
        public abstract string Template { get; }
        public abstract string Description { get; }
        protected abstract IList<string> DefaultValue { get; }

        public void ReceiveValue(IList<string> values)
        {
            if (values == null || values.Count == 0)
            {
                Value = DefaultValue;
                return;
            }

            Value = values.Where(v => !string.IsNullOrEmpty(v)).ToArray();
        }
    }
}