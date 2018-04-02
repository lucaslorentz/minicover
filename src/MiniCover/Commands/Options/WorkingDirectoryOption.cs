using MiniCover.Commands.Options.FileParameterizations;
using System;
using System.IO;

namespace MiniCover.Commands.Options
{
    internal class WorkingDirectoryOption : MiniCoverOption<DirectoryInfo>, IMiniCoverParameterizationOption
    {
        private const string DefaultValue = "./";
        private const string OptionTemplate = "--workdir";

        private static readonly string Description = $"Change working directory [default: {DefaultValue}]";

        internal WorkingDirectoryOption()
            : base(Description, OptionTemplate)
        {
        }

        public Action<MiniCoverParameterization> SetParameter =>
            parameterization =>
                parameterization.WorkDirectory = GetValue();

        protected override DirectoryInfo GetOptionValue()
        {
            var workingDirectoryPath = Option.Value() ?? DefaultValue;
            var directory = Directory.CreateDirectory(workingDirectoryPath);
            Console.WriteLine($"Changing working directory to '{directory.FullName}'");
            Directory.SetCurrentDirectory(directory.FullName);
            return directory;
        }

        protected override bool Validation() => true;
    }
}