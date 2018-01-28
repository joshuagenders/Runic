using System.Collections.Immutable;

namespace Runic.Agent.Core.Models
{
    public class MethodStepInformation
    {
        public MethodStepInformation(
            string assemblyPath, 
            string assemblyQualifiedClassName, 
            string methodName,
            ImmutableList<string> positionalMethodParameterValues = null)
        {
            AssemblyPath = assemblyPath;
            AssemblyQualifiedClassName = assemblyQualifiedClassName;
            MethodName = methodName;
            PositionalMethodParameterValues = positionalMethodParameterValues;
        }

        public string AssemblyPath { get; }
        public string AssemblyQualifiedClassName { get; }
        public string MethodName { get; }
        public ImmutableList<string> PositionalMethodParameterValues { get; }
    }
}
