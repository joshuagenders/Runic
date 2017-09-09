using Runic.Agent.Core.PluginManagement;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.FunctionHarness;
using Runic.Framework.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.Core.StepController;

namespace Runic.Agent.Core.Services
{
    public class RunnerService : IRunnerService
    {
        private readonly IFunctionFactory _functionFactory;
        private readonly IDatetimeService _datetimeService;
        private readonly IPluginManager _pluginManager;
        private readonly IEventService _eventService;

        public RunnerService(IPluginManager pluginManager, IFunctionFactory functionFactory, IDatetimeService datetimeService, IEventService eventService)
        {
            _functionFactory = functionFactory;
            _datetimeService = datetimeService;
            _pluginManager = pluginManager;
            _eventService = eventService;
        }

        public async Task ExecuteFlowAsync(Flow flow, CancellationToken ctx = default(CancellationToken))
        {
            FlowInitialiser flowInitialier = new FlowInitialiser(_pluginManager, _eventService);
            flowInitialier.InitialiseFlow(flow);

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
            _eventService.OnFlowStart(flow);
            try
            {
                while (!ctx.IsCancellationRequested)
                {
                    var step = stepController.GetNextStep(result);
                    if (!string.IsNullOrWhiteSpace(step.Cucumber?.Document))
                    {
                        service = new CucumberStepRunnerService(_pluginManager);
                        var executionResult = await service.ExecuteStepAsync(step, ctx);
                        result = executionResult;
                    }
                    else
                    {
                        service = new FunctionStepRunnerService(_functionFactory, _datetimeService);
                        var executionResult = await service.ExecuteStepAsync(step, ctx);
                        result = executionResult;
                    }
                    _eventService.OnTestResult(result);

                    if (ctx.IsCancellationRequested)
                        break;
                    await _datetimeService.WaitMilliseconds(flow.StepDelayMilliseconds, ctx);
                }
            }
            finally
            {
                _eventService.OnFlowComplete(flow);
            }
        }
    }
}
