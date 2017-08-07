using System.Collections.Generic;

namespace Runic.Agent.Core.Harness
{
    public class CucumberResult : Result
    {
        public string FailedStep { get; set; }
        public List<string> Steps { get; set; }
    }
}
