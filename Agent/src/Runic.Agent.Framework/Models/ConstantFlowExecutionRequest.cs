namespace Runic.Agent.Framework.Models
{
    public class ConstantFlowExecutionRequest
    {
        public string PatternExecutionId { get; set; }
        public ConstantThreadModel ThreadPattern { get; set; }
        public Journey Flow { get; set; }
    }
}