using System;
using System.IO;
using System.Threading;

namespace MiniCover.HitServices
{
    public static class HitService
    {
        public static MethodContext EnterMethod(string fileName)
        {
            return new MethodContext(fileName);
        }

        public class MethodContext
        {
            private readonly string filePath;
            private static readonly AsyncLocal<HitTestMethod> TestMethodCache = new AsyncLocal<HitTestMethod>();

            private readonly HitTestMethod testMethod;
            private readonly bool clearTestMethodCache;

            public MethodContext(string filePath)
            {
                this.filePath = filePath;
                if (TestMethodCache.Value == null)
                {
                    TestMethodCache.Value = HitTestMethod.From(TestMethodUtils.GetTestMethod());
                    clearTestMethodCache = true;
                }

                testMethod = TestMethodCache.Value;
            }

            public void HitInstruction(int id)
            {
                testMethod.HasHit(id);

            }

            public void Exit()
            {
                if (clearTestMethodCache)
                {
                    AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
                    TestMethodCache.Value = null;
                }
            }

            private void CurrentDomain_ProcessExit(object sender, EventArgs e)
            {
                Save();
            }

            private void Save()
            {
                lock (filePath)
                {
                    using (var fileStream = File.Open(this.filePath, FileMode.Append, FileAccess.Write, FileShare.None))
                        testMethod.Serialize(fileStream);
                }
            }
        }
    }
}