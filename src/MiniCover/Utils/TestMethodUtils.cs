using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MiniCover.Utils
{
    public static class TestMethodUtils
    {
        private static readonly ConcurrentDictionary<string, bool> assemblyHasPdbCache = new ConcurrentDictionary<string, bool>();

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

            return assemblyHasPdbCache.GetOrAdd(location, l => File.Exists(Path.ChangeExtension(location, ".pdb")));
        }
    }
}