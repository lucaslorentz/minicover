using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using MiniCover.Extensions;
using MiniCover.HitServices;
using MiniCover.Model;
using MiniCover.Utils;
using Mono.Cecil;

namespace MiniCover.Instrumentation
{
    public class AssemblyInstrumenter
    {
        private static readonly ConstructorInfo instrumentedAttributeConstructor = typeof(InstrumentedAttribute).GetConstructors().First();

        private readonly TypeInstrumenter _typeInstrumenter;
        private readonly ILogger<AssemblyInstrumenter> _logger;
        private readonly ILogger<CustomAssemblyResolver> _assemblyResolverLogger;

        public AssemblyInstrumenter(
            TypeInstrumenter typeInstrumenter,
            ILogger<AssemblyInstrumenter> logger,
            ILogger<CustomAssemblyResolver> assemblyResolverLogger)
        {
            _typeInstrumenter = typeInstrumenter;
            _logger = logger;
            _assemblyResolverLogger = assemblyResolverLogger;
        }

        public InstrumentedAssembly InstrumentAssemblyFile(
            InstrumentationContext context,
            FileInfo assemblyFile)
        {
            var assemblyDirectory = assemblyFile.Directory;

            using (_logger.BeginScope("Checking assembly file {assembly}", assemblyFile.FullName, LogLevel.Information))
            {
                var resolver = new CustomAssemblyResolver(assemblyDirectory, _assemblyResolverLogger);

                _logger.LogTrace("Assembly resolver search directories: {directories}", new object[] { resolver.GetSearchDirectories() });

                try
                {
                    using (var assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyFile.FullName, new ReaderParameters { ReadSymbols = true, AssemblyResolver = resolver }))
                    {
                        return InstrumentAssemblyDefinition(context, assemblyDefinition);
                    }
                }
                catch (BadImageFormatException)
                {
                    _logger.LogInformation("Invalid assembly format");
                    return null;
                }
            }
        }

        private InstrumentedAssembly InstrumentAssemblyDefinition(
            InstrumentationContext context,
            AssemblyDefinition assemblyDefinition)
        {
            if (assemblyDefinition.CustomAttributes.Any(a => a.AttributeType.Name == "InstrumentedAttribute"))
            {
                _logger.LogInformation("Already instrumented");
                return null;
            }

            var assemblyDocuments = assemblyDefinition.GetAllDocuments();

            var changedDocuments = assemblyDocuments.Where(d => d.FileHasChanged()).ToArray();
            if (changedDocuments.Any())
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    var changedFiles = changedDocuments.Select(d => d.Url).Distinct().ToArray();
                    _logger.LogDebug("Source files has changed: {changedFiles}", new object[] { changedFiles });
                }
                else
                {
                    _logger.LogInformation("Source files has changed");
                }
                return null;
            }

            if (!assemblyDocuments.Any(d => context.IsSource(d.Url) || context.IsTest(d.Url)))
            {
                _logger.LogInformation("No link to source files or test files");
                return null;
            }

            _logger.LogInformation("Instrumenting");

            var instrumentedAssembly = new InstrumentedAssembly(assemblyDefinition.Name.Name);
            var instrumentedAttributeReference = assemblyDefinition.MainModule.ImportReference(instrumentedAttributeConstructor);
            assemblyDefinition.CustomAttributes.Add(new CustomAttribute(instrumentedAttributeReference));

            foreach (var type in assemblyDefinition.MainModule.GetTypes())
            {
                _typeInstrumenter.InstrumentType(
                    context,
                    type,
                    instrumentedAssembly);
            }

            var miniCoverTempPath = GetMiniCoverTempPath();

            var instrumentedAssemblyFile = new FileInfo(Path.Combine(miniCoverTempPath, $"{Guid.NewGuid()}.dll"));
            var instrumentedPdbFile = FileUtils.GetPdbFile(instrumentedAssemblyFile);

            assemblyDefinition.Write(instrumentedAssemblyFile.FullName, new WriterParameters { WriteSymbols = true });

            instrumentedAssembly.TempAssemblyFile = instrumentedAssemblyFile.FullName;
            instrumentedAssembly.TempPdbFile = instrumentedPdbFile.FullName;

            return instrumentedAssembly;
        }

        private static string GetMiniCoverTempPath()
        {
            var path = Path.Combine(Path.GetTempPath(), "minicover");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}
