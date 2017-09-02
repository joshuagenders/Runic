namespace Runic.Agent.Aws.Configuration
{
    public interface IAgentSettings
    {
        string FlowPatternExecutionId { get; }
        string AgentFlowFilepath { get; }
        string FlowThreadPatternName { get; }
        string AgentPluginDirectory { get; }
        string[] FlowPoints { get; }
        int FlowDurationSeconds { get; }
        int FlowThreadCount { get; }
        int FlowRampUpSeconds { get; }
        int FlowRampDownSeconds { get; }
        int FlowStepIntervalSeconds { get; }
    }
}
