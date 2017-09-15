using Runic.Agent.TestHarness.StepController;

namespace Runic.Agent.TestHarness.Harness
{
    public class FunctionResult : Result
    {
        public string FunctionName { get; set; }
        public string StepName { get; set; }
        public object[] FunctionParameters { get; set; }
    }
}
