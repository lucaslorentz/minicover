using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace MiniCover.HitServices
{
    public static class HitService
    {
        static ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();

        public static MethodContext EnterMethod(string fileName)
        {
            return new MethodContext(fileName);
        }

        public class MethodContext
        {
            private readonly string filePath;
            private static readonly AsyncLocal<HitTestMethod> TestMethodCache = new AsyncLocal<HitTestMethod>();
            private static Dictionary<string, Stream> filesStream = new Dictionary<string, Stream>();

            private readonly HitTestMethod testMethod;
            private readonly bool clearTestMethodCache;

            public MethodContext(string filePath)
            {
                this.filePath = filePath;
                if (TestMethodCache.Value == null)
                {
                    var currentUri = new Uri(Directory.GetCurrentDirectory());
                    TestMethodCache.Value = HitTestMethod.From(TestMethodUtils.GetTestMethod(), currentUri);
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
                    Save();
                    TestMethodCache.Value = null;
                }
            }

            private void Save()
            {
                rwl.EnterWriteLock();
                try
                {
                    using(var fileStream = GetFileStream())
                    {
                        testMethod.Serialize(fileStream);
                        fileStream.Flush();
                        fileStream.Close();
                    }
                }
                finally
                {
                    rwl.ExitWriteLock();
                }
            }

            private Stream GetFileStream()
            {
                return File.Open(this.filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            }
        }
    }
}