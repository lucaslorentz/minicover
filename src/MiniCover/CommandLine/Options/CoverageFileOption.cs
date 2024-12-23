using System.IO.Abstractions;

namespace MiniCover.CommandLine.Options
{
    public class CoverageFileOption : FileOption
    {
        public CoverageFileOption(IFileSystem fileSystem)
            : base(fileSystem)
        {
        }

        public override string Name => "--coverage-file";
        public override string Description => $"Coverage file name [default: {DefaultValue}]";
        protected override string DefaultValue => "./coverage.json";
    }
}