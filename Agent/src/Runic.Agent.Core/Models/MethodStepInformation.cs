using System.Collections.Generic;

namespace Runic.Agent.Core.Models
{
    public class MethodStepInformation
    {
        public MethodStepInformation(
            string assemblyName, 
            string assemblyQualifiedClassName, 
            string methodName,
            List<string> positionalMethodParameterValues = null)
        {
            AssemblyName = assemblyName;
            AssemblyQualifiedClassName = assemblyQualifiedClassName;
            MethodName = methodName;
            PositionalMethodParameterValues = positionalMethodParameterValues;
        }

        public string AssemblyName { get; }
        public string AssemblyQualifiedClassName { get; }
        public string MethodName { get; }
        public List<string> PositionalMethodParameterValues { get; }
    }
}
