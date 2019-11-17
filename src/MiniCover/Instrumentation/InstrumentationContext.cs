using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MiniCover.Instrumentation
{
    public class InstrumentationContext
    {
        public virtual IList<FileInfo> Assemblies { get; set; }
        public virtual IList<FileInfo> Sources { get; set; }
        public virtual IList<FileInfo> Tests { get; set; }
        public virtual string HitsPath { get; set; }
        public virtual DirectoryInfo Workdir { get; set; }
        public virtual int InstructionId { get; set; }

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
