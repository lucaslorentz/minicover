using System;
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

            public void HitInstruction(int id)
            {
                _hitContext.RecordHit(id);
            }

            public void Dispose()
            {
                if (_saveHitContext)
                {
                    Directory.CreateDirectory(_hitsPath);
                    var filePath = Path.Combine(_hitsPath, $"{Guid.NewGuid()}.hits");
                    using (var fileStream = File.Open(filePath, FileMode.CreateNew))
                    {
                        _hitContext.Serialize(fileStream);
                        fileStream.Flush();
                    }
                    HitContext.Current = null;
                }
            }
        }
    }
}