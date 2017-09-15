using Runic.Agent.TestHarness.StepController;
using System.Collections.Generic;

namespace Runic.Agent.TestHarness.Harness
{
    public class CucumberResult : Result
    {
        public string FailedStep { get; set; }
        public List<string> Steps { get; set; }
    }
}
