using Config.Net;

namespace Runic.Agent.Standalone.Configuration
{
    public class StatsdSettings : SettingsContainer
    {
        public readonly Option<int> StatsdPort;
        public readonly Option<string> StatsdHost;
        public readonly Option<string> StatsdPrefix;

        protected override void OnConfigure(IConfigConfiguration configuration)
        {
            configuration.UseCommandLineArgs();
        }
    }
}
