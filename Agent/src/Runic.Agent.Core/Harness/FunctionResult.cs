using Runic.Agent.StepController;

namespace Runic.Agent.Harness
{
    public class FunctionResult : Result
    {
        public string MethodName { get; set; }
        public string StepName { get; set; }
        public object[] FunctionParameters { get; set; }
    }
}
