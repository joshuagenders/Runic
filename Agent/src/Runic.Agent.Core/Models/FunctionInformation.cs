using System.Collections.Generic;

namespace Runic.Agent.Core.Models
{
    public class MethodInformation
    {
        public string AssemblyName { get; set; }
        public string AssemblyQualifiedClassName { get; set; }
        public string MethodName { get; set; }
        public List<string> PositionalMethodParameterValues { get; set; }
        public bool GetNextStepFromMethodResult { get; set; }
    }
}
