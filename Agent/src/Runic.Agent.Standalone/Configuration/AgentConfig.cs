namespace Runic.Agent.Standalone.Configuration
{
    public class AgentConfig
    {
        private static StatsdSettings _statsdSettings = new StatsdSettings();
        public static StatsdSettings StatsdSettings => _statsdSettings;

        private static AgentSettings _agentSettings = new AgentSettings();
        public static AgentSettings AgentSettings => _agentSettings;
    }
}