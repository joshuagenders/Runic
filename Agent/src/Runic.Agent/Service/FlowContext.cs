namespace Runic.Agent.Service
{
    public class FlowContext
    {
        public string FlowName { get; set; }
        public int ThreadCount { get; set; }
        public int RunningThreadCount { get; set; }
    }
}