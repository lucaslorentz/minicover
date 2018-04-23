using MiniCover.Utils;
using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace MiniCover.Instrumentation
{
    public class CustomAssemblyResolver : DefaultAssemblyResolver
    {
        public CustomAssemblyResolver() : base()
        {
            RemoveSearchDirectory(".");
            RemoveSearchDirectory("bin");
        }

        AssemblyDefinition GetAssembly(string file, ReaderParameters parameters)
        {
            if (parameters.AssemblyResolver == null)
            {
                parameters.AssemblyResolver = this;
            }

            return ModuleDefinition.ReadModule(file, parameters).Assembly;
        }

        protected override AssemblyDefinition SearchDirectory(AssemblyNameReference name, IEnumerable<string> directories, ReaderParameters parameters)
        {
            foreach (var directory in directories)
            {
                var files = FileUtils.GetFiles(null, null, $"**/{name.Name}.dll", directory);
                foreach (var file in files)
                {
                    try
                    {
                        return GetAssembly(file, parameters);
                    }
                    catch (BadImageFormatException)
                    {
                        continue;
                    }
                }
            }
            return null;
        }
    }
}
