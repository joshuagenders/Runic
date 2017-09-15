using Runic.Agent.Framework.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.TestHarness.StepController;
using Runic.Agent.TestHarness.Harness;
using System.Reflection;

namespace Runic.Agent.TestHarness.Services
{
    public class RunnerService : IRunnerService
    {
        private readonly IFunctionFactory _functionFactory;
        private readonly IDatetimeService _datetimeService;
        private readonly Assembly _assembly;
        
        public RunnerService(IFunctionFactory functionFactory, IDatetimeService datetimeService, Assembly assembly)
        {
            _functionFactory = functionFactory;
            _datetimeService = datetimeService;
            _assembly = assembly;
        }

        public async Task ExecuteFlowAsync(Flow flow, CancellationToken ctx = default(CancellationToken))
        {
            Result result = null;
            IStepRunnerService service;
            IStepController stepController;
            if (flow.Steps.All(s => s.Distribution != null))
            {
                stepController = new DistributionStepController(flow.Steps);
            }
            else
            {
                stepController = new StandardStepController(flow);
            }
            try
            {
                while (!ctx.IsCancellationRequested)
                {
                    var step = stepController.GetNextStep(result);
                    if (!string.IsNullOrWhiteSpace(step.Cucumber?.Document))
                    {
                        service = new CucumberStepRunnerService(_assembly);
                        var executionResult = await service.ExecuteStepAsync(step, ctx);
                        result = executionResult;
                    }
                    else
                    {
                        service = new FunctionStepRunnerService(_functionFactory, _datetimeService);
                        var executionResult = await service.ExecuteStepAsync(step, ctx);
                        result = executionResult;
                    }
                    //_eventService.OnTestResult(result);

                    if (ctx.IsCancellationRequested)
                        break;
                    await _datetimeService.WaitMilliseconds(flow.StepDelayMilliseconds, ctx);
                }
            }
            finally
            {
                //_eventService.OnFlowComplete(flow);
            }
        }
    }
}
