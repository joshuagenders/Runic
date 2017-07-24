namespace Runic.Agent.Standalone.Configuration
{
    public interface IAgentConfig
    {
        IStatsdSettings StatsdSettings { get; }
        IAgentSettings AgentSettings { get; }
    }
}
