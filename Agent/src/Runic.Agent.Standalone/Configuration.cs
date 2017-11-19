using Runic.Agent.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Runic.Agent.Standalone
{
    public class Configuration : ICoreConfiguration
    {
        public int TaskCreationPollingIntervalSeconds => throw new NotImplementedException();
    }
}
