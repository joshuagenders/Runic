using System;
using System.Collections.Generic;

namespace Runic.Framework.Models
{
    public class FunctionInformation
    {
        public string AssemblyName { get; set; }
        public string AssemblyQualifiedClassName { get; set; }
        public string FunctionName { get; set; }
        public Dictionary<string,Type> Parameters { get; set; }
        public List<string> RequiredRunes { get; set; } 
    }
}
