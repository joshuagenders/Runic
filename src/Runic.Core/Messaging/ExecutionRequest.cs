namespace Runic.Core.Messaging
{
    public class ExecutionRequest
    {
        public string TestAssemblyName { get; set; }
        public string TestName { get; set; }
        public int TimeoutMilliseconds { get; set; }
        public int StepDelayMilliseconds { get; set; }
        public bool ContinueOnStepFailure { get; set; }
    }
}
