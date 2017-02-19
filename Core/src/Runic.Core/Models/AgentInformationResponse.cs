using System;
using System.Collections.Generic;

namespace Runic.Core.Models
{
    public class AgentInformationResponse
    {
        public List<Test> Tests { get; set; }
    }

    public class Test
    {
        public Type TestType { get; set; }
        public List<List<Dictionary<string, Type>>> MutableParameters { get; set; }
        public List<string> RequiredRunes { get; set; }
        public List<string> RequiredLinkedRunes { get; set; }
    }
}