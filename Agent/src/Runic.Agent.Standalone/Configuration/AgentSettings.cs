using Config.Net;

namespace Runic.Agent.Standalone.Configuration
{
    public class AgentSettings : SettingsContainer
    {
        public readonly Option<string> PatternExecutionId;
        public readonly Option<string> FlowFilepath;
        public readonly Option<string> ThreadPatternName;

        //graph
        public readonly Option<string[]> Points;
        //constant+graph
        public readonly Option<int> DurationSeconds;
        //constant+gradual
        public readonly Option<int> ThreadCount;
        //gradual
        public readonly Option<int> RampUpSeconds; 
        public readonly Option<int> RampDownSeconds; 
        public readonly Option<int> StepIntervalSeconds; 

        protected override void OnConfigure(IConfigConfiguration configuration)
        {
            configuration.UseCommandLineArgs();
        }
    }
}
