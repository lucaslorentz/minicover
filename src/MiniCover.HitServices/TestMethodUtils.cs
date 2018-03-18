using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MiniCover.HitServices
{
    public static class TestMethodUtils
    {
        private static readonly IEnumerable<string> TestFrameworkAssemblies = new[]
        {
            "NUnit3.TestAdapter",
            "nunit.framework",
            "xunit.core",
            "xunit.execution.dotnet",
            "Microsoft.TestPlatform.Core.Utilities",
            "Microsoft.TestPlatform.CrossPlatEngine",
            "Microsoft.VisualStudio.TestPlatform.MSTest.TestAdpater",
            "Microsoft.VisualStudio.TestPlatform.TestFramework"
        };
        private static readonly Dictionary<string, bool> AssemblyHasPdbCache = new Dictionary<string, bool>();
        private static readonly object Lock = new object();
        public static MethodBase GetTestMethod()
        {
            var stackTrace = new StackTrace();

            var frames = stackTrace.GetFrames();
            for (int i = frames.Length - 1; i >= 0; i--)
            {
                var currentMethod = frames[i].GetMethod();
                if (HasPdb(currentMethod))
                    return currentMethod;
            }

            return null;
        }

        private static bool HasPdb(MethodBase methodBase)
        {
            var location = methodBase.DeclaringType.Assembly.Location;
            
            lock (Lock)
            {
                if (AssemblyHasPdbCache.TryGetValue(location, out var hasPdb)) return hasPdb;
                hasPdb = IsNotATestFrameworkAssembly(methodBase.DeclaringType.Assembly) && File.Exists(Path.ChangeExtension(location, ".pdb"));
                AssemblyHasPdbCache.Add(location, hasPdb);
                return hasPdb;
            }
        }

        private static bool IsNotATestFrameworkAssembly(Assembly assembly)
        {
            return !TestFrameworkAssemblies.Any(framework => assembly.GetName().Name.Equals(framework, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}