namespace Runic.Framework.Models
{
    public class GraphFlowExecutionRequest
    {
        public string ThreadPatternName { get; set; }
        public GraphThreadModel ThreadPattern { get; set; }
        public Flow Flow { get; set; }
    }
}