using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using MiniCover.Core.Extensions;
using Mono.Cecil;

namespace MiniCover.Core.Instrumentation
{
    public class FileBasedInstrumentationContext : IInstrumentationContext
    {
        private int _uniqueId;

        public virtual IList<IFileInfo> Assemblies { get; set; }
        public virtual IList<IFileInfo> Sources { get; set; }
        public virtual IList<IFileInfo> Tests { get; set; }
        public virtual string HitsPath { get; set; }
        public virtual IDirectoryInfo Workdir { get; set; }

        public virtual int NewInstructionId()
        {
            return ++_uniqueId;
        }

        public virtual bool IsSource(MethodDefinition methodDefinition)
        {
            return methodDefinition.GetAllDocuments()
                .Any(d => Sources.Any(s => s.FullName == d.Url));
        }

        public virtual bool IsTest(MethodDefinition methodDefinition)
        {
            return methodDefinition.GetAllDocuments()
                .Any(d => Tests.Any(s => s.FullName == d.Url));
        }
    }
}
