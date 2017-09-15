namespace Runic.Agent.Framework.Models
{
    public class SetThreadLevelRequest
    {
        public string FlowId { get; set; }
        public string FlowName { get; set; }
        public int ThreadLevel { get; set; }
    }
}
