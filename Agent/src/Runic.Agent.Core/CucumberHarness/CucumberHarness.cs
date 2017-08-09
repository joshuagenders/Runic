﻿using System.Threading.Tasks;
using Runic.Cucumber;
using Runic.Agent.Core.AssemblyManagement;
using System.Threading;
using System.Diagnostics;

namespace Runic.Agent.Core.CucumberHarness
{
    public class CucumberHarness
    {
        private readonly IPluginManager _pluginManager;
        public CucumberHarness(IPluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }

        public async Task<CucumberResult> ExecuteTestAsync(string assemblyName, string document, CancellationToken ctx = default(CancellationToken))
        {
            _pluginManager.LoadPlugin(assemblyName);
            var assembly = _pluginManager.GetPlugin(assemblyName);
            var test = new CucumberTest(assembly);

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var testTask = test.ExecuteAsync(document, ctx);
            var result = await testTask;
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
