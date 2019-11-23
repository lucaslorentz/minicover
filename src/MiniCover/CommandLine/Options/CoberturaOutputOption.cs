namespace MiniCover.CommandLine.Options
{
    class CoberturaOutputOption : FileOption
    {
        private const string _defaultValue = "./cobertura.xml";
        private const string _template = "--output";
        private static readonly string _description = $"Output file for cobertura report [default: {_defaultValue}]";

        public CoberturaOutputOption()
            : base(_template, _description)
        {
        }

        protected override string GetDefaultValue()
        {
            return _defaultValue;
        }
    }
}