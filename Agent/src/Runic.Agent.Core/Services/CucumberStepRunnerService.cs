using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Services.Interfaces;
using Runic.Framework.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.Services
{
    public class CucumberStepRunnerService : IStepRunnerService
    {
        private readonly CucumberHarness.CucumberHarness _harness;
        public CucumberStepRunnerService(IPluginManager pluginManager)
        {
            _harness = new CucumberHarness.CucumberHarness(pluginManager);
        }

        public async Task<Result> ExecuteStepAsync(Step step, CancellationToken ctx = default(CancellationToken))
        {
            return await _harness.ExecuteTestAsync(step.Cucumber.AssemblyName, step.Cucumber.Document, ctx);
        }
    }
}
