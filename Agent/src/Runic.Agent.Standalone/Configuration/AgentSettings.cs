using Config.Net;

namespace Runic.Agent.Standalone.Configuration
{
    public class AgentSettings : SettingsContainer
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

        protected override void OnConfigure(IConfigConfiguration configuration)
        {
            configuration.UseCommandLineArgs();
        }
    }
}
