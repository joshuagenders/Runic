using Runic.Agent.Core.Models;
using Runic.Cucumber;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Runic.Agent.Core.Harness
{
    public class CucumberHarness
    {
        public async Task<Result> ExecuteTestAsync(Assembly assembly, Step step)
        {
            var test = new CucumberTest(assembly);

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var result = await test.ExecuteAsync(step.Cucumber.Document);
            stopWatch.Stop();

            return new Result(result.Success, stopWatch.ElapsedMilliseconds, result.Exception?.Message, step);
        }
    }
}
