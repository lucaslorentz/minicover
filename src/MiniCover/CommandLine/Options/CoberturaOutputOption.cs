using System.IO.Abstractions;

namespace MiniCover.CommandLine.Options
{
    public class CoberturaOutputOption : FileOption, ICoberturaOutputOption
    {
        public CoberturaOutputOption(IFileSystem fileSystem)
            : base(fileSystem)
        {
        }

        public override string Template => "--output";
        public override string Description => $"Output file for cobertura report [default: {DefaultValue}]";
        protected override string DefaultValue => "./cobertura.xml";
    }
}