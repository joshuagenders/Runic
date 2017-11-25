using Runic.Agent.Core.Harness;

namespace Runic.Agent.Core.Harness
{
    public class FunctionResult : Result
    {
        public string MethodName { get; set; }
        public string StepName { get; set; }
        public object[] FunctionParameters { get; set; }
    }
}
