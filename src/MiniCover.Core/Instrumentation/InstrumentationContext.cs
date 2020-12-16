using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace MiniCover.Core.Instrumentation
{
    public class InstrumentationContext
    {
        public virtual IList<IFileInfo> Assemblies { get; set; }
        public virtual IList<IFileInfo> Sources { get; set; }
        public virtual IList<IFileInfo> Tests { get; set; }
        public virtual string HitsPath { get; set; }
        public virtual IDirectoryInfo Workdir { get; set; }
        public virtual int UniqueId { get; set; }

        public virtual bool IsSource(string file)
        {
            return Sources.Any(s => s.FullName == file);
        }

        public virtual bool IsTest(string file)
        {
            return Tests.Any(s => s.FullName == file);
        }
    }
}
