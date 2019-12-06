using System;
using System.Collections.Concurrent;
using System.IO;

namespace MiniCover.HitServices
{
    public static class HitService
    {
        public static MethodContext EnterMethod(
            string hitsPath,
            string assemblyName,
            string className,
            string methodName)
        {
            return new MethodContext(hitsPath, assemblyName, className, methodName);
        }

        public class MethodContext : IDisposable
        {
            private static ConcurrentDictionary<string, Stream> _filesStream = new ConcurrentDictionary<string, Stream>();

            private readonly string _hitsPath;
            private readonly HitContext _hitContext;
            private readonly bool _saveHitContext;

            public MethodContext(
                string hitsPath,
                string assemblyName,
                string className,
                string methodName)
            {
                _hitsPath = hitsPath;

                if (HitContext.Current == null)
                {
                    _hitContext = new HitContext(assemblyName, className, methodName);
                    HitContext.Current = _hitContext;
                    _saveHitContext = true;
                }
                else
                {
                    _hitContext = HitContext.Current;
                }
            }

            public void Hit(int id)
            {
                _hitContext.RecordHit(id);
            }

            public void Dispose()
            {
                if (_saveHitContext)
                {
                    var fileStream = _filesStream.GetOrAdd(_hitsPath, CreateOutputFile);
                    lock (fileStream)
                    {
                        _hitContext.Serialize(fileStream);
                        fileStream.Flush();
                    }
                    HitContext.Current = null;
                }
            }

            private static FileStream CreateOutputFile(string hitsPath)
            {
                Directory.CreateDirectory(hitsPath);
                var filePath = Path.Combine(hitsPath, $"{Guid.NewGuid()}.hits");
                return File.Open(filePath, FileMode.CreateNew);
            }
        }
    }
}