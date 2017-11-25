using Runic.Cucumber;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.Harness
{
    public class CucumberHarness
    {
        public async Task<CucumberResult> ExecuteTestAsync(Assembly assembly, string document, CancellationToken ctx = default(CancellationToken))
        {
            var test = new CucumberTest(assembly);

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = await test.ExecuteAsync(document, ctx);
            stopWatch.Stop();

            return new CucumberResult()
            {
                Exception = result.Exception,
                FailedStep = result.FailedStep,
                Steps = result.Steps,
                Success = result.Success,
                ExecutionTimeMilliseconds = stopWatch.ElapsedMilliseconds
            };
        }
    }
}
