namespace Runic.Agent.Core.Harness
{
    public class FunctionResult : Result
    {
        public string FunctionName { get; set; }
        public string StepName { get; set; }
        public string NextStep { get; set; }
        public object[] FunctionParameters { get; set; }
    }
}
