using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using MiniCover.Core.Utils;
using Mono.Cecil;

namespace MiniCover.Core.Instrumentation
{
    public class CustomAssemblyResolver : DefaultAssemblyResolver
    {
        private readonly DependencyContext _dependencyContext;
        private readonly ILogger _logger;

        public CustomAssemblyResolver(
            IDirectoryInfo assemblyDirectory,
            ILogger logger,
            DepsJsonUtils depsJsonUtils)
        {
            _logger = logger;

            foreach (var searchDirectory in GetSearchDirectories())
                RemoveSearchDirectory(searchDirectory);

            AddSearchDirectory(assemblyDirectory.FullName);

            _dependencyContext = depsJsonUtils.LoadDependencyContext(assemblyDirectory);

            var runtimeConfigPath = assemblyDirectory.GetFiles("*.runtimeconfig.dev.json", SearchOption.TopDirectoryOnly)
                .Concat(assemblyDirectory.GetFiles("*.runtimeconfig.json", SearchOption.TopDirectoryOnly))
                .FirstOrDefault();

            if (runtimeConfigPath != null)
            {
                var runtimeConfigContent = File.ReadAllText(runtimeConfigPath.FullName);
                foreach (var path in depsJsonUtils.GetAdditionalPaths(runtimeConfigContent))
                {
                    AddSearchDirectory(path);
                }
            }
        }

        private AssemblyDefinition GetAssembly(string file, ReaderParameters parameters)
        {
            if (parameters.AssemblyResolver == null)
            {
                parameters.AssemblyResolver = this;
            }

            return ModuleDefinition.ReadModule(file, parameters).Assembly;
        }

        protected override AssemblyDefinition SearchDirectory(AssemblyNameReference name, IEnumerable<string> directories, ReaderParameters parameters)
        {
            if (_dependencyContext != null)
            {
                foreach (var library in _dependencyContext.RuntimeLibraries)
                {
                    foreach (var runtimeAssemblyGroup in library.RuntimeAssemblyGroups)
                    {
                        foreach (var runtimeFile in runtimeAssemblyGroup.RuntimeFiles)
                        {
                            var runtimeAssemblyName = Path.GetFileNameWithoutExtension(runtimeFile.Path);

                            if (!runtimeAssemblyName.Equals(name.Name, StringComparison.InvariantCultureIgnoreCase))
                                continue;

                            if (runtimeFile.AssemblyVersion != null
                                && runtimeFile.AssemblyVersion != name.Version.ToString()
                                && runtimeFile.FileVersion != null
                                && runtimeFile.FileVersion != name.Version.ToString())
                                continue;

                            foreach (var directory in directories)
                            {
                                var file = directory;

                                if (!string.IsNullOrEmpty(library.Path))
                                    file = Path.Combine(file, Path.Combine(library.Path.Split('/')));

                                file = Path.Combine(file, Path.Combine(runtimeFile.Path.Split('/')));

                                _logger.LogTrace("Trying to load file {file}", file);

                                if (File.Exists(file))
                                {
                                    try
                                    {
                                        return GetAssembly(file, parameters);
                                    }
                                    catch (BadImageFormatException ex)
                                    {
                                        _logger.LogError(ex, "Faile to read assembly {file}", file);
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                _logger.LogTrace("Dependency context is null");
            }

            _logger.LogTrace("Fallback to DefaultAssemblyResolver");

            return base.SearchDirectory(name, directories, parameters);
        }
    }
}
