using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MiniCover
{
    public class HitTestMethod
    {
        internal HitTestMethod(MethodBase testMethod)
        {
            AssemblyName = testMethod.DeclaringType.Assembly.FullName;
            ClassName = testMethod.DeclaringType.Name;
            MethodName = testMethod.Name;
            AssemblyLocation = testMethod.DeclaringType.Assembly.Location;
        }

        [JsonConstructor]

        internal HitTestMethod(string assemblyName,
            string className,
            string methodName,
            string assemblyLocation,
            int counter)
        {
            AssemblyName = assemblyName;
            ClassName = className;
            MethodName = methodName;
            AssemblyLocation = assemblyLocation;
            Counter = counter;
        }

        public string AssemblyName { get; }
        public string ClassName { get; }
        public string MethodName { get; }
        public string AssemblyLocation { get; }
        public int Counter { get; private set; }

        public void Hited()
        {
            Counter++;
        }

        public static IList<HitTestMethod> MergeDuplicates(IEnumerable<HitTestMethod> source)
        {
            return source
                .GroupBy(h => new { h.AssemblyName, h.ClassName, h.MethodName, h.AssemblyLocation })
                .Select(g => new HitTestMethod(
                    g.Key.AssemblyName,
                    g.Key.ClassName,
                    g.Key.MethodName,
                    g.Key.AssemblyLocation,
                    g.Sum(h => h.Counter)
                )).ToArray();
        }
    }
}