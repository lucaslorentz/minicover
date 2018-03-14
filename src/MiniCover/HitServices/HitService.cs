using MiniCover.Utils;
using System.IO;
using System.Threading;

namespace MiniCover
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
                    TestMethodCache.Value = new HitTestMethod(TestMethodUtils.GetTestMethod());
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
                    this.Save();
                    TestMethodCache.Value = null;
                }
            }

            private void Save()
            {
                using (var fileStream = File.Open(this.filePath, FileMode.Append, FileAccess.Write, FileShare.None))
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    if (fileStream.Position != 0) streamWriter.Write(",");
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(testMethod);
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }
            }
        }
    }
}