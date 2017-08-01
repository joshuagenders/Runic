using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Runic.Cucumber;
using Runic.Agent.Core.AssemblyManagement;

namespace Runic.Agent.Core.Harness
{
    public class CucumberHarness
    {
        private readonly IPluginManager _pluginManager;
        public CucumberHarness(IPluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }
        public async Task ExecuteTestAsync(string assemblyName, string document)
        {
            var test = new CucumberTest(_pluginManager.LoadPlugin(assemblyName));
        }
    }
}
