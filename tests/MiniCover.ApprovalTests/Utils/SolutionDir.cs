using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.IO;
using System.Linq;

namespace MiniCover.ApprovalTests.Utils
{
    internal class SolutionDir
    {
        public static string GetRootPath()
        {
            var type = typeof(SolutionDir);

            IAssemblyResolver GetAssemblyResolver(string assemblyLocation)
            {
                var testResolver = new DefaultAssemblyResolver();
                var directory = Path.GetDirectoryName(assemblyLocation);
                testResolver.AddSearchDirectory(directory);
                return testResolver;
            }

            var parameters = new ReaderParameters
            {
                SymbolReaderProvider = new PortablePdbReaderProvider(),
                AssemblyResolver = GetAssemblyResolver(new Uri(type.Assembly.CodeBase).LocalPath),
                ReadSymbols = true,
                ReadWrite = false,
                ReadingMode = ReadingMode.Deferred
            };
            var module = ModuleDefinition.ReadModule(type.Assembly.Location, parameters);
            var document = module.GetType(type.FullName).Methods
                .First(a => a.DebugInformation.HasSequencePoints).DebugInformation.SequencePoints.First().Document.Url;
            return new Uri(Path.Combine(Path.GetDirectoryName(document), "../../../" )).LocalPath;
        }
    }
}