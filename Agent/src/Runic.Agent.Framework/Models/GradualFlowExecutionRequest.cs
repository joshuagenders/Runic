namespace Runic.Agent.Framework.Models
{
    public class GradualFlowExecutionRequest
    {
        public string PatternExecutionId { get; set; }
        public GradualThreadModel ThreadPattern { get; set; }
        public Journey Flow { get; set; }
    }
}