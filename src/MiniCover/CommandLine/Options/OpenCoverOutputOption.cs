using System.IO.Abstractions;

namespace MiniCover.CommandLine.Options
{
    public class OpenCoverOutputOption : FileOption, IOpenCoverOutputOption
    {
        public OpenCoverOutputOption(IFileSystem fileSystem)
            : base(fileSystem)
        {
        }

        public override string Name => "--output";
        public override string Description => $"Output file for OpenCover report [default: {DefaultValue}]";
        protected override string DefaultValue => "./opencovercoverage.xml";
    }
}