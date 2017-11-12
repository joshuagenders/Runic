using System;
using System.Collections.Generic;

namespace Runic.Agent.Framework.Models
{
    public class FunctionInformation
    {
        public string AssemblyName { get; set; }
        public string AssemblyQualifiedClassName { get; set; }
        public string FunctionName { get; set; }
        public List<string> PositionalMethodParameterValues { get; set; }
        public bool GetNextStepFromFunctionResult { get; set; }
    }
}
