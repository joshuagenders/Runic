namespace Runic.Agent.Framework.Models
{
    public class GraphFlowExecutionRequest
    {
        public string PatternExecutionId { get; set; }
        public GraphThreadModel ThreadPattern { get; set; }
        public Flow Flow { get; set; }
    }
}