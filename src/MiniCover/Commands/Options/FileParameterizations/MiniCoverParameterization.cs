using MiniCover.Model;
using System.IO;

namespace MiniCover.Commands.Options.FileParameterizations
{
    internal class MiniCoverParameterization
    {
        public InstrumentationResult InstrumentationResult { get; set; }
        public string OpenCoverFile { get; set; }
        public float Threshold { get; set; }
        public DirectoryInfo WorkDirectory { get; set; }
    }
}