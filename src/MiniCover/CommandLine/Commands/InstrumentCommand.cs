using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using MiniCover.CommandLine;
using MiniCover.CommandLine.Options;
using MiniCover.Exceptions;
using MiniCover.Instrumentation;
using MiniCover.Model;
using MiniCover.Utils;
using Newtonsoft.Json;

namespace MiniCover.Commands
{
    class InstrumentCommand : BaseCommand
    {
        private const string _name = "instrument";
        private const string _description = "Instrument assemblies";

        private readonly IServiceProvider _serviceProvider;
        private readonly WorkingDirectoryOption _workingDirectoryOption;
        private readonly ParentDirectoryOption _parentDirOption;
        private readonly IncludeAssembliesPatternOption _includeAssembliesOption;
        private readonly ExcludeAssembliesPatternOption _excludeAssembliesOption;
        private readonly IncludeSourcesPatternOption _includeSourceOption;
        private readonly ExcludeSourcesPatternOption _excludeSourceOption;
        private readonly IncludeTestsPatternOption _includeTestsOption;
        private readonly ExcludeTestsPatternOption _excludeTestsOption;
        private readonly HitsDirectoryOption _hitsDirectoryOption;
        private readonly CoverageFileOption _coverageFileOption;

        public InstrumentCommand(IServiceProvider serviceProvider,
            VerbosityOption verbosityOption,
            WorkingDirectoryOption workingDirectoryOption,
            ParentDirectoryOption parentDirOption,
            IncludeAssembliesPatternOption includeAssembliesOption,
            ExcludeAssembliesPatternOption excludeAssembliesOption,
            IncludeSourcesPatternOption includeSourceOption,
            ExcludeSourcesPatternOption excludeSourceOption,
            IncludeTestsPatternOption includeTestsOption,
            ExcludeTestsPatternOption excludeTestsOption,
            HitsDirectoryOption hitsDirectoryOption,
            CoverageFileOption coverageFileOption)
            : base(_name, _description)
        {
            _serviceProvider = serviceProvider;
            _workingDirectoryOption = workingDirectoryOption;
            _parentDirOption = parentDirOption;
            _includeAssembliesOption = includeAssembliesOption;
            _excludeAssembliesOption = excludeAssembliesOption;
            _includeSourceOption = includeSourceOption;
            _excludeSourceOption = excludeSourceOption;
            _includeTestsOption = includeTestsOption;
            _excludeTestsOption = excludeTestsOption;
            _hitsDirectoryOption = hitsDirectoryOption;
            _coverageFileOption = coverageFileOption;

            Options = new IOption[]
            {
                verbosityOption,
                workingDirectoryOption,
                parentDirOption,
                includeAssembliesOption,
                excludeAssembliesOption,
                includeSourceOption,
                excludeSourceOption,
                includeTestsOption,
                excludeTestsOption,
                hitsDirectoryOption,
                coverageFileOption
            };
        }

        protected override Task<int> Execute()
        {
            var assemblies = GetFiles(_includeAssembliesOption.Value, _excludeAssembliesOption.Value, _parentDirOption.Value);
            if (assemblies.Length == 0)
                throw new ValidationException("No assemblies found");

            var sourceFiles = GetFiles(_includeSourceOption.Value, _excludeSourceOption.Value, _parentDirOption.Value);
            if (sourceFiles.Length == 0)
                throw new ValidationException("No source files found");

            var testFiles = GetFiles(_includeTestsOption.Value, _excludeTestsOption.Value, _parentDirOption.Value);

            var instrumentationContext = new InstrumentationContext
            {
                Assemblies = assemblies,
                HitsPath = _hitsDirectoryOption.Value.FullName,
                Sources = sourceFiles,
                Tests = testFiles,
                Workdir = _workingDirectoryOption.Value.AddEndingDirectorySeparator()
            };

            var instrumenter = _serviceProvider.GetService<Instrumenter>();
            var result = instrumenter.Execute(instrumentationContext);

            var coverageFile = _coverageFileOption.Value;
            SaveCoverageFile(coverageFile, result);

            return Task.FromResult(0);
        }

        private static FileInfo[] GetFiles(
            IEnumerable<string> includes,
            IEnumerable<string> excludes,
            DirectoryInfo parentDir)
        {
            var matcher = new Microsoft.Extensions.FileSystemGlobbing.Matcher();

            foreach (var include in includes)
            {
                matcher.AddInclude(include);
            }

            foreach (var exclude in excludes)
            {
                matcher.AddExclude(exclude);
            }

            var fileMatchResult = matcher.Execute(new DirectoryInfoWrapper(parentDir));

            return fileMatchResult.Files
                .Select(f => new FileInfo(Path.GetFullPath(Path.Combine(parentDir.ToString(), f.Path))))
                .ToArray();
        }

        private static void SaveCoverageFile(FileInfo coverageFile, InstrumentationResult result)
        {
            var settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };
            var json = JsonConvert.SerializeObject(result, Formatting.Indented, settings);
            File.WriteAllText(coverageFile.FullName, json);
        }
    }
}
