namespace Runic.Framework.Models
{
    public class ConstantFlowExecutionRequest
    {
        public string ThreadPatternName { get; set; }
        public ConstantThreadModel ThreadPattern { get; set; }
        public Flow Flow { get; set; }
    }
}