using System;
using System.IO;
using System.Reflection;
using Mono.Cecil.Cil;

using Xunit;

namespace Mono.Cecil.Tests
{

    public abstract class BaseTestFixture
    {
        public static string GetResourcePath(string name, Assembly assembly)
        {
            return Path.Combine(FindResourcesDirectory(assembly), name);
        }

        public static string GetAssemblyResourcePath(string name, Assembly assembly)
        {
            return GetResourcePath(Path.Combine("assemblies", name), assembly);
        }

        public static string GetCSharpResourcePath(string name, Assembly assembly)
        {
            return GetResourcePath(Path.Combine("cs", name), assembly);
        }

        public static string GetILResourcePath(string name, Assembly assembly)
        {
            return GetResourcePath(Path.Combine("il", name), assembly);
        }

        public ModuleDefinition GetResourceModule(string name)
        {
            return ModuleDefinition.ReadModule(GetAssemblyResourcePath(name, GetType().Assembly));
        }

        public ModuleDefinition GetResourceModule(string name, ReaderParameters parameters)
        {
            return ModuleDefinition.ReadModule(GetAssemblyResourcePath(name, GetType().Assembly), parameters);
        }

        public ModuleDefinition GetResourceModule(string name, ReadingMode mode)
        {
            return ModuleDefinition.ReadModule(GetAssemblyResourcePath(name, GetType().Assembly), new ReaderParameters(mode));
        }

        public ModuleDefinition GetCurrentModule()
        {
            return ModuleDefinition.ReadModule(GetType().Module.FullyQualifiedName);
        }

        public ModuleDefinition GetCurrentModule(ReaderParameters parameters)
        {
            return ModuleDefinition.ReadModule(GetType().Module.FullyQualifiedName, parameters);
        }

        public static string FindResourcesDirectory(Assembly assembly)
        {
            var path = Path.GetDirectoryName(new Uri(assembly.CodeBase).LocalPath);
            while (!Directory.Exists(Path.Combine(path, "Resources")))
            {
                var old = path;
                path = Path.GetDirectoryName(path);
                Assert.NotEqual(old, path);
            }

            return Path.Combine(path, "Resources");
        }

        public static void AssertCode(string expected, MethodDefinition method)
        {
            Assert.True(method.HasBody);
            Assert.NotNull(method.Body);

            Assert.Equal(Normalize(expected), Normalize(Formatter.FormatMethodBody(method)));
        }

        public static string Normalize(string str)
        {
            return str.Trim().Replace("\r\n", "\n").Replace("\t", "");
        }

        public static void TestModule(string file, Action<ModuleDefinition> test, Type symbolReaderProvider = null,
            IAssemblyResolver assemblyResolver = null, bool applyWindowsRuntimeProjections = false)
        {
            Run(new ModuleTestCase(file, test, symbolReaderProvider, assemblyResolver, applyWindowsRuntimeProjections));
        }

        public static void TestCSharp(string file, Action<ModuleDefinition> test, Type symbolReaderProvider = null,
            IAssemblyResolver assemblyResolver = null, bool applyWindowsRuntimeProjections = false)
        {
            Run(new CSharpTestCase(file, test, symbolReaderProvider, assemblyResolver, applyWindowsRuntimeProjections));
        }

        public static void TestIL(string file, Action<ModuleDefinition> test, Type symbolReaderProvider = null,
            IAssemblyResolver assemblyResolver = null, bool applyWindowsRuntimeProjections = false)
        {
            Run(new ILTestCase(file, test, symbolReaderProvider, assemblyResolver, applyWindowsRuntimeProjections));
        }

        static void Run(TestCase testCase)
        {
            using (var runner = new TestRunner(testCase, TestCaseType.ReadImmediate))
                runner.RunTest();
        }
    }

    abstract class TestCase
    {

        public readonly Type SymbolReaderProvider;
        public readonly IAssemblyResolver AssemblyResolver;
        public readonly Action<ModuleDefinition> Test;
        public readonly bool ApplyWindowsRuntimeProjections;

        public abstract string ModuleLocation { get; }

        protected Assembly Assembly => Test.Method.Module.Assembly;

        protected TestCase(Action<ModuleDefinition> test, Type symbolReaderProvider, IAssemblyResolver assemblyResolver, bool applyWindowsRuntimeProjections)
        {
            Test = test;
            SymbolReaderProvider = symbolReaderProvider;
            AssemblyResolver = assemblyResolver;
            ApplyWindowsRuntimeProjections = applyWindowsRuntimeProjections;
        }
    }

    class ModuleTestCase : TestCase
    {
        public readonly string Module;

        public ModuleTestCase(string module, Action<ModuleDefinition> test, Type symbolReaderProvider, IAssemblyResolver assemblyResolver, bool applyWindowsRuntimeProjections)
            : base(test, symbolReaderProvider, assemblyResolver, applyWindowsRuntimeProjections)
        {
            Module = module;
        }

        public override string ModuleLocation => BaseTestFixture.GetAssemblyResourcePath(Module, Assembly);
    }

    class CSharpTestCase : TestCase
    {

        public readonly string File;

        public CSharpTestCase(string file, Action<ModuleDefinition> test, Type symbolReaderProvider, IAssemblyResolver assemblyResolver, bool applyWindowsRuntimeProjections)
            : base(test, symbolReaderProvider, assemblyResolver, applyWindowsRuntimeProjections)
        {
            File = file;
        }

        public override string ModuleLocation => CompilationService.CompileResource(BaseTestFixture.GetCSharpResourcePath(File, Assembly));
    }

    class ILTestCase : TestCase
    {
        public readonly string File;

        public ILTestCase(string file, Action<ModuleDefinition> test, Type symbolReaderProvider, IAssemblyResolver assemblyResolver, bool applyWindowsRuntimeProjections)
            : base(test, symbolReaderProvider, assemblyResolver, applyWindowsRuntimeProjections)
        {
            File = file;
        }

        public override string ModuleLocation => CompilationService.CompileResource(BaseTestFixture.GetILResourcePath(File, Assembly));
    }

    class TestRunner : IDisposable
    {

        readonly TestCase testCase;
        readonly TestCaseType type;

        ModuleDefinition testModule;
        DefaultAssemblyResolver testResolver;

        public TestRunner(TestCase testCase, TestCaseType type)
        {
            this.testCase = testCase;
            this.type = type;
        }

        ModuleDefinition GetModule()
        {
            var location = testCase.ModuleLocation;

            var parameters = new ReaderParameters
            {
                SymbolReaderProvider = GetSymbolReaderProvider(),
                AssemblyResolver = GetAssemblyResolver(),
                ApplyWindowsRuntimeProjections = testCase.ApplyWindowsRuntimeProjections
            };

            switch (type)
            {
                case TestCaseType.ReadImmediate:
                    parameters.ReadingMode = ReadingMode.Immediate;
                    return ModuleDefinition.ReadModule(location, parameters);
                default:
                    return null;
            }
        }

        ISymbolReaderProvider GetSymbolReaderProvider()
        {
            if (testCase.SymbolReaderProvider == null)
                return null;

            return (ISymbolReaderProvider)Activator.CreateInstance(testCase.SymbolReaderProvider);
        }

        IAssemblyResolver GetAssemblyResolver()
        {
            if (testCase.AssemblyResolver != null)
                return testCase.AssemblyResolver;

            testResolver = new DefaultAssemblyResolver();
            var directory = Path.GetDirectoryName(testCase.ModuleLocation);
            testResolver.AddSearchDirectory(directory);
            return testResolver;
        }

        public void RunTest()
        {
            var module = GetModule();
            if (module == null)
                return;

            testModule = module;
            testCase.Test(module);
        }

        public void Dispose()
        {
            testModule?.Dispose();

            testResolver?.Dispose();
        }
    }

    enum TestCaseType
    {
        ReadImmediate,
        ReadDeferred,
    }
}
