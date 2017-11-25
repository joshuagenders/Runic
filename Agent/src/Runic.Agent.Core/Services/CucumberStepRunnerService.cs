using Runic.Agent.StepController;
using Runic.Agent.Core.Models;
using Runic.Agent.Harness;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Services
{
    public class CucumberStepRunnerService : IStepRunnerService
    {
        private readonly CucumberHarness _harness;
        private readonly Assembly _assembly;

        public CucumberStepRunnerService(Assembly assembly)
        {
            _harness = new CucumberHarness();
            _assembly = assembly;
        }

        public async Task<Result> ExecuteStepAsync(Step step, CancellationToken ctx = default(CancellationToken))
        {
            var result = await _harness.ExecuteTestAsync(_assembly, step.Cucumber.Document, ctx);
            return result;
        }
    }
}
