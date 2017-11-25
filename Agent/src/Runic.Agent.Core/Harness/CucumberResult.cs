using Runic.Agent.StepController;
using System.Collections.Generic;

namespace Runic.Agent.Harness
{
    public class CucumberResult : Result
    {
        public string FailedStep { get; set; }
        public List<string> Steps { get; set; }
    }
}
