using Runic.Agent.Core.StepController;

namespace Runic.Agent.Core.FunctionHarness
{
    public class FunctionResult : Result
    {
        public string FunctionName { get; set; }
        public string StepName { get; set; }
        public object[] FunctionParameters { get; set; }
    }
}
