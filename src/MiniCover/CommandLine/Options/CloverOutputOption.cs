using System.IO.Abstractions;

namespace MiniCover.CommandLine.Options
{
    public class CloverOutputOption : FileOption, ICloverOutputOption
    {
        public CloverOutputOption(IFileSystem fileSystem)
            : base(fileSystem)
        {
        }

        public override string Template => "--output";
        public override string Description => $"Output file for Clover report [default: {DefaultValue}]";
        protected override string DefaultValue => "./clover.xml";
    }
}