using Config.Net;

namespace Runic.Agent.Aws.Configuration
{
    public class StatsdSettings : SettingsContainer, IStatsdSettings
    {
        private readonly Option<int> StatsdPort = new Option<int>(8125);
        private readonly Option<string> StatsdHost = new Option<string>("localhost");
        private readonly Option<string> StatsdPrefix = new Option<string>("Runic");

        public string Prefix => StatsdPrefix;
        public int Port => StatsdPort;
        public string Host => StatsdHost;

        protected override void OnConfigure(IConfigConfiguration configuration)
        {
            configuration.UseCommandLineArgs();
        }
    }
}
