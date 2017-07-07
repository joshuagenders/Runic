﻿using Config.Net;

namespace Runic.Agent.Standalone.Configuration
{
    public class StatsdSettings : SettingsContainer
    {
        public readonly Option<int> StatsdPort = new Option<int>(8125);
        public readonly Option<string> StatsdHost = new Option<string>("localhost");
        public readonly Option<string> StatsdPrefix = new Option<string>("Runic");

        protected override void OnConfigure(IConfigConfiguration configuration)
        {
            configuration.UseCommandLineArgs();
            //todo josh pass command line args into plugin as context
        }
    }
}
