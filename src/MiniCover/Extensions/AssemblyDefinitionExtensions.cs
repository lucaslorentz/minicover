using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniCover.Extensions
{
    public static class AssemblyDefinitionExtensions
    {
        public static IEnumerable<MethodDefinition> GetAllMethods(this AssemblyDefinition assemblyDefinition)
        {
            foreach (var type in assemblyDefinition.MainModule.GetTypes())
            {
                foreach (var method in type.GetAllMethods())
                {
                    yield return method;
                }
            }
        }
    }
}
