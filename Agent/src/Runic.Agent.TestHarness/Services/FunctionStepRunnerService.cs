using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.Framework.Models;
using Runic.Agent.TestHarness.StepController;
using Runic.Agent.TestHarness.Harness;

namespace Runic.Agent.TestHarness.Services
{
    public class FunctionStepRunnerService : IStepRunnerService
    {
        private readonly IFunctionFactory _functionFactory;
       
        public FunctionStepRunnerService(IFunctionFactory functionFactory, IDatetimeService datetimeService)
        {
            _functionFactory = functionFactory;
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
