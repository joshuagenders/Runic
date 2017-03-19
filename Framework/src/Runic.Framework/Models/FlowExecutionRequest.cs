namespace Runic.Framework.Models
{
    public class FlowExecutionRequest
    {
        public string FlowName { get; set; }
        public int Threads { get; set; }
        public int RampUpMinutes { get; set; }
        public int RampDownMinutes { get; set; }
        public int ExecutionLengthMinutes { get; set; }
        public Flow Flow { get; set; }
    }
}