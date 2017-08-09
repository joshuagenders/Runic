using System.Threading;
using System.Threading.Tasks;
using Runic.Framework.Models;
using Runic.Agent.Core.FunctionHarness;
using Runic.Agent.Core.Services.Interfaces;

namespace Runic.Agent.Core.Services
{
    public class FunctionStepRunnerService : IStepRunnerService
    {
        private readonly IFunctionFactory _functionFactory;
        private readonly IDatetimeService _datetimeService;

        public FunctionStepRunnerService(IFunctionFactory functionFactory, IDatetimeService datetimeService)
        {
            _functionFactory = functionFactory;
            _datetimeService = datetimeService;
        }

        public async Task<Result> ExecuteStepAsync(Step step, CancellationToken ctx = default(CancellationToken))
        {
            //TODO test context
            var testContext = new TestContext();
            var function = _functionFactory.CreateFunction(step, testContext);
            return await function.OrchestrateFunctionExecutionAsync(ctx);
        }
        
    }
}
