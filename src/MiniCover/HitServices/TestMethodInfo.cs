using MiniCover.HitServices;
using MiniCover.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MiniCover
{
    [Serializable]
    public class TestMethodInfo : IEquatable<TestMethodInfo>
    {
        private static ConcurrentDictionary<string, bool> assemblyHasPdbCache = new ConcurrentDictionary<string, bool>();

        public string AssemblyName { get; }
        public string ClassName { get; }
        public string MethodName { get; }
        public string AssemblyLocation { get; }
        public int Counter { get; private set; }

        public static TestMethodInfo Build(string assemblyName, string className, string methodName, string assemblyLocation) 
            => new  TestMethodInfo(assemblyName, className, methodName, assemblyLocation, 1);
        
        public TestMethodInfo(string assemblyName, string className, string methodName, string assemblyLocation, int counter)
        {
            AssemblyName = assemblyName;
            ClassName = className;
            MethodName = methodName;
            AssemblyLocation = assemblyLocation;
            Counter = counter;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TestMethodInfo);
        }

        public bool Equals(TestMethodInfo other)
        {
            return other != null &&
                   AssemblyName == other.AssemblyName &&
                   ClassName == other.ClassName &&
                   MethodName == other.MethodName &&
                   AssemblyLocation == other.AssemblyLocation;
        }

        public override int GetHashCode()
        {
            var hashCode = -1427917565;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AssemblyName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ClassName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MethodName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AssemblyLocation);
            return hashCode;
        }

        public void HasCall()
        {
            this.Counter++;
        }

        public static TestMethodInfo GetCurrentTestMethodInfo()
        {

            if (Current == null)
            {
                var testMethod = OptimizedStackTrace.GetTestMethod(HasPdb);
                Current = Create(testMethod);
            }

            return Current;
        }

        internal TestMethodInfo Clone()
        {
            return Build(this.AssemblyName, this.ClassName, this.MethodName, this.AssemblyLocation);
        }

        //private static MethodBase GetTestMethod()
        //{

        //    var stackTrace = new StackTrace();

        //    var frames = stackTrace.GetFrames();
        //    for (int i = frames.Length - 1; i >= 0; i--)
        //    {
        //        var currentMethod = frames[i].GetMethod();
        //        if (HasPdb(currentMethod))
        //            return currentMethod;
        //    }

        //    return null;
        //}

        private static bool HasPdb(MethodBase methodBase)
        {
            var location = methodBase.DeclaringType.Assembly.Location;
            return assemblyHasPdbCache.GetOrAdd(location, l => File.Exists(Path.ChangeExtension(location, ".pdb")));
        }
        public static TestMethodInfo Current
        {
            get => HitContext.Get();
            set => HitContext.Set(value);
        }
        public static TestMethodInfo Create(MethodBase testMethod)
        {
            return Build(testMethod.DeclaringType.Assembly.FullName, testMethod.DeclaringType.Name, testMethod.Name, testMethod.DeclaringType.Assembly.Location);
        }

        public static TestMethodInfo Merge(IEnumerable<TestMethodInfo> methodsToMerge)
        {
            var result = methodsToMerge.First();
            var counter =   methodsToMerge.Sum(a => a.Counter);
            result.Counter = counter;
            return result;
        }
    }
}