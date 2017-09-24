using System;
using Runic.Agent.Core.Configuration;

namespace Runic.Agent.Standalone.Configuration
{
    public class AgentConfig : IAgentConfig, ICoreConfiguration
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

        public int MaxActivePopulation => 400;
        public int MaxErrors => 200;
    }
}