namespace Runic.Agent.Framework.Models
{
    public class TestContext
    {
        public string FlowExecutionId { get; set; }
        public string FlowName { get; set; }
        public string ThreadPatternName{ get; set; }
        
        public string PluginDirectory { get; set; }
        public string DeploymentDirectory { get; set; }
        public string ResultsDirectory { get; set; }
    }
}
