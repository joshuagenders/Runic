namespace Runic.Agent.Core.Models
{
    public class FunctionResult : Result
    {
        public string MethodName { get; set; }
        public string StepName { get; set; }
        public object[] FunctionParameters { get; set; }
    }
}
