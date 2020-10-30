using System;
using System.IO;

namespace MiniCover.HitServices
{
    public class MethodScope : IDisposable
    {
        private readonly HitContext _hitContext;
        private readonly string _hitsPath;
        private readonly bool _isEntryMethod;

        public MethodScope(
            string hitsPath,
            string assemblyName,
            string className,
            string methodName)
        {
            _hitContext = HitContext.Current;

            if (_hitContext == null)
            {
                _hitContext = new HitContext(assemblyName, className, methodName);
                _isEntryMethod = true;
                HitContext.Current = _hitContext;
            }

            _hitsPath = hitsPath;

            lock (_hitContext)
            {
                _hitContext.EnterMethod();
            }
        }

        public void Hit(int id)
        {
            lock (_hitContext)
            {
                _hitContext.RecordHit(id);
            }
        }

        public void Dispose()
        {
            if (_isEntryMethod)
                HitContext.Current = null;

            lock (_hitContext)
            {
                if (_hitContext.ExitMethod() == 0)
                    SaveHitContext();
            }
        }

        private void SaveHitContext()
        {
            lock (_hitContext)
            {
                if (_hitContext.Hits.Count == 0)
                    return;

                Directory.CreateDirectory(_hitsPath);

                var fileName = Path.Combine(_hitsPath, $"{_hitContext.Id}.hits");

                using (var fileStream = File.Open(fileName, FileMode.Create))
                {
                    _hitContext.Serialize(fileStream);
                    fileStream.Flush();
                }
            }
        }
    }
}
