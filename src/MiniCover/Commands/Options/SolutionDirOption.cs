using System;
using System.IO;

namespace MiniCover.Commands.Options
{
    internal class SolutionDirOption : MiniCoverOption<DirectoryInfo>
    {
        private const string DefaultValue = "./";
        public override string Description => $"Solution directory [default: {DefaultValue}]";
        public override string OptionTemplate => "--solutiondir";

        public override void Invalidate()
        {
            var solutionPath = Option.Value() ?? DefaultValue;
            if (!Directory.Exists(solutionPath))
            {
                throw new ArgumentException($"Solution directory does not exist '{solutionPath}'");
            }

            Invalidated = true;
            ValueField = new DirectoryInfo(solutionPath);
        }
    }
}