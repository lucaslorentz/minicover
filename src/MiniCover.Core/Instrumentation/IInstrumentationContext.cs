using System.Collections.Generic;
using System.IO.Abstractions;
using Mono.Cecil;

namespace MiniCover.Core.Instrumentation
{
    public interface IInstrumentationContext
    {
        IList<IFileInfo> Assemblies { get; }
        string HitsPath { get; }
        IDirectoryInfo Workdir { get; }
        int NewInstructionId();
        bool IsSource(MethodDefinition methodDefinition);
        bool IsTest(MethodDefinition methodDefinition);
    }
}
