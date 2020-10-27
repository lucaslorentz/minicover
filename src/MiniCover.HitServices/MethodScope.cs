using System;
using System.IO;

namespace MiniCover.HitServices
{
    public class MethodScope : IDisposable
    {
        private readonly HitContext _hitContext;
        private readonly string _hitsPath;
        private readonly bool _createdHitContext;

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
                _createdHitContext = true;
                HitContext.Current = _hitContext;
            }

            _hitsPath = hitsPath;
            _hitContext.IncrementRef();
        }

        public void Hit(int id)
        {
            _hitContext.RecordHit(id);
        }

        public void Dispose()
        {
            if (_hitContext.DecrementRef() == 0)
                SaveHitContext();

            if (_createdHitContext)
                HitContext.Current = null;
        }

        private void SaveHitContext()
        {
            Directory.CreateDirectory(_hitsPath);

            var fileName = Path.Combine(_hitsPath, $"{Guid.NewGuid()}.hits");

            using (var fileStream = File.Open(fileName, FileMode.OpenOrCreate))
            {
                _hitContext.Serialize(fileStream);
                fileStream.Flush();
            }
        }
    }
}
