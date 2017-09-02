namespace Runic.Agent.Aws.Configuration
{
    public interface IAgentConfig
    {
        IStatsdSettings StatsdSettings { get; }
        IAgentSettings AgentSettings { get; }
    }
}
