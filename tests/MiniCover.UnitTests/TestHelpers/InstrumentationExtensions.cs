using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using MiniCover.Extensions;
using MiniCover.Infrastructure.FileSystem;
using MiniCover.Instrumentation;
using MiniCover.Model;
using Mono.Cecil;

namespace MiniCover.UnitTests.TestHelpers
{
    public static class InstrumentationExtensions
    {
        public static AssemblyDefinition ToDefinition(this Assembly assembly)
        {
            var assemblyFile = new FileInfo(assembly.Location);
            var resolver = new CustomAssemblyResolver(assemblyFile.Directory, NullLogger<CustomAssemblyResolver>.Instance);
            var readerParameters = new ReaderParameters { ReadSymbols = true, AssemblyResolver = resolver };
            return AssemblyDefinition.ReadAssembly(assemblyFile.FullName, readerParameters);
        }

        public static TypeDefinition ToDefinition(this Type type)
        {
            var assemblyDefinition = type.Assembly.ToDefinition();
            return (TypeDefinition)assemblyDefinition.MainModule.LookupToken(type.MetadataToken);
        }

        public static MethodDefinition ToDefinition(this MethodBase method)
        {
            var assemblyDefinition = method.DeclaringType.Assembly.ToDefinition();
            return (MethodDefinition)assemblyDefinition.MainModule.LookupToken(method.MetadataToken);
        }

        public static InstrumentedAssembly Instrument(this TypeDefinition typeDefinition)
        {
            var documents = typeDefinition.GetAllDocuments();
            var instrumentationContext = CreateInstrumentationContext(documents);
            var instrumentedAssembly = new InstrumentedAssembly(typeDefinition.Module.Assembly.Name.Name);
            var methodInstrumenter = CreateMethodInstrumenter();
            var typeInstrumenter = new TypeInstrumenter(methodInstrumenter);
            typeInstrumenter.InstrumentType(instrumentationContext, typeDefinition, instrumentedAssembly);
            return instrumentedAssembly;
        }

        public static InstrumentedAssembly Instrument(this MethodDefinition methodDefinition)
        {
            var documents = methodDefinition.GetAllDocuments();
            var instrumentationContext = CreateInstrumentationContext(documents);
            var instrumentedAssembly = new InstrumentedAssembly(methodDefinition.Module.Assembly.Name.Name);
            var methodInstrumenter = CreateMethodInstrumenter();
            methodInstrumenter.InstrumentMethod(instrumentationContext, true, methodDefinition, instrumentedAssembly);
            return instrumentedAssembly;
        }

        public static Type Load(this TypeDefinition typeDefinition)
        {
            using (var ms = new MemoryStream())
            {
                typeDefinition.Module.Assembly.Write(ms);

                var assembly = Assembly.Load(ms.ToArray());

                var typeParts = typeDefinition.FullName.Split("/");

                var type = assembly.GetType(typeParts[0]);
                foreach (var part in typeParts.Skip(1))
                    type = type.GetNestedType(part, BindingFlags.NonPublic | BindingFlags.Public);

                return type;
            }
        }

        public static MethodBase Load(this MethodDefinition methodDefinition)
        {
            var type = methodDefinition.DeclaringType.Load();

            return type.GetMembers().OfType<MethodBase>().First(m => m.Name == methodDefinition.Name);
        }

        private static InstrumentationContext CreateInstrumentationContext(IEnumerable<string> documents)
        {
            return new InstrumentationContext
            {
                HitsPath = "/tmp",
                Workdir = new DirectoryInfo("/tmp"),
                Sources = documents.Select(d => new FileInfo(d)).ToArray(),
                Tests = new FileInfo[0]
            };
        }

        private static MethodInstrumenter CreateMethodInstrumenter()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var fileReader = new CachedFileReader(memoryCache);
            var methodInstrumenter = new MethodInstrumenter(NullLogger<MethodInstrumenter>.Instance, fileReader);
            return methodInstrumenter;
        }
    }
}
