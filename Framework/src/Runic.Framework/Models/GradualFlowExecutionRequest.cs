namespace Runic.Framework.Models
{
    public class GradualFlowExecutionRequest
    {
        public string ThreadPatternName { get; set; }
        public GradualThreadModel ThreadPattern { get; set; }
        public Flow Flow { get; set; }
    }
}