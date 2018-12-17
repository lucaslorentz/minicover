using Microsoft.Extensions.DependencyModel;
using MiniCover.Utils;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MiniCover.Instrumentation
{
    public class CustomAssemblyResolver : DefaultAssemblyResolver
    {
        private DependencyContext _dependencyContext;

        public CustomAssemblyResolver(string assemblyDirectory)
        {
            RemoveSearchDirectory(".");
            RemoveSearchDirectory("bin");

            AddSearchDirectory(assemblyDirectory);

            _dependencyContext = DepsJsonUtils.LoadDependencyContext(assemblyDirectory);

            var runtimeConfigPath = Directory.GetFiles(assemblyDirectory, "*.runtimeconfig.dev.json", SearchOption.TopDirectoryOnly)
                .FirstOrDefault();

            if (runtimeConfigPath != null)
            {
                var runtimeConfigContent = File.ReadAllText(runtimeConfigPath);
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

                                Console.WriteLine($"Try to load file {file}");

                                if (File.Exists(file))
                                {
                                    try
                                    {
                                        return GetAssembly(file, parameters);
                                    }
                                    catch (BadImageFormatException)
                                    {
                                        Console.WriteLine($"BadImageFormatException!");
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"DependencyContext.RuntimeLibraries. No information about assembly {name.Name}!");
                }
            }
            else
            {
                Console.WriteLine("Dependency context is null!");
            }

            Console.WriteLine("base SearchDirectory");
            return base.SearchDirectory(name, directories, parameters);
        }
    }
}
