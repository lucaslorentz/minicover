using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using MiniCover.Utils;
using Mono.Cecil;

namespace MiniCover.Instrumentation
{
    public class CustomAssemblyResolver : DefaultAssemblyResolver
    {
        private readonly DependencyContext _dependencyContext;
        private readonly ILogger _logger;

        public CustomAssemblyResolver(DirectoryInfo assemblyDirectory, ILogger logger)
        {
            _logger = logger;

            RemoveSearchDirectory(".");
            RemoveSearchDirectory("bin");

            AddSearchDirectory(assemblyDirectory.FullName);

            _dependencyContext = DepsJsonUtils.LoadDependencyContext(assemblyDirectory);

            var runtimeConfigPath = assemblyDirectory.GetFiles("*.runtimeconfig.dev.json", SearchOption.TopDirectoryOnly)
                .FirstOrDefault();

            if (runtimeConfigPath != null)
            {
                var runtimeConfigContent = File.ReadAllText(runtimeConfigPath.FullName);
                foreach (var path in DepsJsonUtils.GetAdditionalPaths(runtimeConfigContent))
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
                var library = _dependencyContext.RuntimeLibraries.FirstOrDefault(c =>
                {
                    return c.Name == name.Name;
                });

                if (library != null)
                {
                    foreach (var runtimeAssemblyGroup in library.RuntimeAssemblyGroups)
                    {
                        foreach (var runtimeAssemblyPath in runtimeAssemblyGroup.AssetPaths)
                        {
                            foreach (var directory in directories)
                            {
                                var file = directory;

                                if (!string.IsNullOrEmpty(library.Path))
                                    file = Path.Combine(file, Path.Combine(library.Path.Split("/")));

                                file = Path.Combine(file, Path.Combine(runtimeAssemblyPath.Split("/")));

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
                else
                {
                    _logger.LogWarning("DependencyContext.RuntimeLibraries. No information about assebmly {name}", name.Name);
                }
            }
            else
            {
                _logger.LogWarning("Dependency context is null");
            }

            _logger.LogTrace("Fallback to DefaultAssemblyResolver");

            return base.SearchDirectory(name, directories, parameters);
        }
    }
}
