using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.Core.Models;
using Runic.Agent.StepController;
using Runic.Agent.Harness;

namespace Runic.Agent.Services
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
            return await function.ExecuteAsync(ctx);
        }
        
    }
}
