using System.IO.Abstractions;

namespace MiniCover.CommandLine.Options
{
    public class NCoverOutputOption : FileOption, INCoverOutputOption
    {
        public NCoverOutputOption(IFileSystem fileSystem)
            : base(fileSystem)
        {
        }

        public override string Name => "--output";
        public override string Description => $"Output file for NCover report [default: {DefaultValue}]";
        protected override string DefaultValue => "./coverage.xml";
    }
}