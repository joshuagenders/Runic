namespace Runic.Agent.Aws.Configuration
{
    public class AgentConfig : IAgentConfig
    {
        public AgentConfig(IStatsdSettings statsdSettings, IAgentSettings agentSettings)
        {
            _statsdSettings = statsdSettings;
            _agentSettings = agentSettings;
        }

        private static IStatsdSettings _statsdSettings { get; set; }
        public IStatsdSettings StatsdSettings => _statsdSettings;

        private static IAgentSettings _agentSettings { get; set; }
        public IAgentSettings AgentSettings => _agentSettings;
    }
}