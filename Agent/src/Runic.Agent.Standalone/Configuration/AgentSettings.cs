using Config.Net;

namespace Runic.Agent.Standalone.Configuration
{
    public class AgentSettings : SettingsContainer, IAgentSettings
    {
        public readonly Option<string> PatternExecutionId;
        public readonly Option<string> FlowFilepath;
        public readonly Option<string> ThreadPatternName;
        public readonly Option<string> PluginDirectory = new Option<string>("Plugins");

        //graph
        public readonly Option<string[]> Points;
        //constant+graph
        public readonly Option<int> DurationSeconds;
        //constant+gradual
        public readonly Option<int> ThreadCount = new Option<int>(1);
        //gradual
        public readonly Option<int> RampUpSeconds = new Option<int>(0);
        public readonly Option<int> RampDownSeconds = new Option<int>(0); 
        public readonly Option<int> StepIntervalSeconds = new Option<int>(1);

        public string FlowPatternExecutionId => PatternExecutionId;
        public string AgentFlowFilepath => FlowFilepath;
        public string FlowThreadPatternName => ThreadPatternName;
        public string AgentPluginDirectory => PluginDirectory;
        public string[] FlowPoints => Points;
        public int FlowDurationSeconds => DurationSeconds;
        public int FlowThreadCount => ThreadCount;
        public int FlowRampUpSeconds => RampUpSeconds;
        public int FlowRampDownSeconds => RampDownSeconds;
        public int FlowStepIntervalSeconds => StepIntervalSeconds;

        protected override void OnConfigure(IConfigConfiguration configuration)
        {
            configuration.UseCommandLineArgs();
        }
    }
}
