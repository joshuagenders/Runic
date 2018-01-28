using System.Collections.Immutable;

namespace Runic.Agent.Core.Models
{
    public class MethodStepInformation
    {
        public MethodStepInformation(
            string assemblyName, 
            string assemblyQualifiedClassName, 
            string methodName,
            ImmutableList<string> positionalMethodParameterValues = null)
        {
            AssemblyName = assemblyName;
            AssemblyQualifiedClassName = assemblyQualifiedClassName;
            MethodName = methodName;
            PositionalMethodParameterValues = positionalMethodParameterValues;
        }

        public string AssemblyName { get; }
        public string AssemblyQualifiedClassName { get; }
        public string MethodName { get; }
        public ImmutableList<string> PositionalMethodParameterValues { get; }
    }
}
