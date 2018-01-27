using Runic.Agent.Core.Models;
using Runic.Cucumber;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Runic.Agent.Core.Harness
{
    public class CucumberHarness
    {
        public async Task<Result> ExecuteTestAsync(Assembly assembly, string document)
        {
            var test = new CucumberTest(assembly);

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = await test.ExecuteAsync(document);
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
