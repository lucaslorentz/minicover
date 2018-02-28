using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MiniCover
{
    public static class HitService
    {
        private static readonly object lockObject = new object();
        private static Dictionary<string, StreamWriter> writers = new Dictionary<string, StreamWriter>();

        public static void Init(string fileName)
        {
            lock (lockObject)
            {
                if (!writers.ContainsKey(fileName))
                {
                    writers[fileName] = new StreamWriter(File.Open(fileName, FileMode.Append));
                }
            }
        }

        public static void Hit(string fileName, int id)
        {
            lock (lockObject)
            {
                StackTrace stackTrace = new System.Diagnostics.StackTrace();

                var currentIndex = 0;

                var frames = stackTrace.GetFrames();
                foreach (var currentFrame in frames)
                {
                    if(currentFrame.GetMethod().Name == "InvokeMethod")
                        break;
                    currentIndex++;
                }
                var frame = frames[currentIndex-1];
                var method = frame.GetMethod();
                string methodName = method.Name;
                Type methodClass = method.DeclaringType;
                var streamWriter = writers[fileName];
                streamWriter.WriteLine($"{id}:{methodClass.Assembly.FullName}:{methodClass.FullName}:{methodName}:{methodClass.Assembly.Location}");
                streamWriter.Flush();
            }
        }
    }
}
