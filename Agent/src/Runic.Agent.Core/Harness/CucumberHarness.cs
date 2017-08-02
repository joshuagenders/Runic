using Runic.Agent.Core.AssemblyManagement;
using Runic.Cucumber;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.Harness
{
    public class CucumberHarness
    {
        private readonly IPluginManager _pluginManager;
        public CucumberHarness(IPluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }

        public async Task ExecuteTestAsync(string assemblyName, string document, CancellationToken ctx = default(CancellationToken))
        {
            _pluginManager.LoadPlugin(assemblyName);
            var assembly = _pluginManager.GetPlugin(assemblyName);
            var test = new CucumberTest(assembly);

            var testTask = test.ExecuteAsync(document, ctx);
            await testTask;

            if (testTask.Exception != null)
            {
                throw testTask.Exception;
            }
        }
    }
}
