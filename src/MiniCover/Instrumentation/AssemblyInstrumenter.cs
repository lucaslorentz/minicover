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

        private readonly string[] _loadedAssemblyFiles = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .Select(a => a.Location)
            .ToArray();

        public AssemblyInstrumenter(
            TypeInstrumenter typeInstrumenter,
            ILogger<AssemblyInstrumenter> logger,
            ILogger<CustomAssemblyResolver> assemblyResolverLogger)
        {
            _typeInstrumenter = typeInstrumenter;
            _logger = logger;
            _assemblyResolverLogger = assemblyResolverLogger;
        }

        public InstrumentedAssembly InstrumentAssembly(
            InstrumentationContext context,
            FileInfo assemblyFile)
        {
            var assemblyDirectory = assemblyFile.Directory;

            using (_logger.BeginScope("Checking assembly file {assembly}", assemblyFile.FullName, LogLevel.Information))
            {
                if (assemblyFile.Name == "MiniCover.HitServices.dll")
                {
                    _logger.LogInformation("Skipping HitServices");
                    return null;
                }

                if (_loadedAssemblyFiles.Contains(assemblyFile.FullName))
                {
                    _logger.LogInformation("Can't instrument loaded assembly");
                    return null;
                }

                var resolver = new CustomAssemblyResolver(assemblyDirectory, _assemblyResolverLogger);

                _logger.LogTrace("Assembly resolver search directories: {directories}", new object[] { resolver.GetSearchDirectories() });

                using (var assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyFile.FullName, new ReaderParameters { ReadSymbols = true, AssemblyResolver = resolver }))
                {
                    if (assemblyDefinition.CustomAttributes.Any(a => a.AttributeType.Name == "InstrumentedAttribute"))
                    {
                        _logger.LogInformation("Already instrumented");
                        return null;
                    }

                    var assemblyDocuments = assemblyDefinition.GetAllDocuments();
                    if (!assemblyDocuments.Any(d => context.IsSource(d) || context.IsTest(d)))
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
            }
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
