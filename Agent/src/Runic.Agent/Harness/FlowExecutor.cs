using NLog;
using Runic.Agent.AssemblyManagement;
using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Harness
{
    public class FlowExecutor
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private Dictionary<string, Assembly> _assemblies { get; set; }
        private readonly PluginManager _pluginManager;
        private readonly Flow _flow;

        public FlowExecutor(Flow flow, PluginManager pluginManager)
        {
            _pluginManager = pluginManager;
            _flow = flow;
        }
        public async Task ExecuteFlow(CancellationToken ctx = default(CancellationToken))
        {
            var navigator = new FlowNavigator(_flow, _pluginManager);
            FunctionHarness function = null;
            bool lastStepSuccess = false;
            while (!ctx.IsCancellationRequested)
            {
                function = navigator.GetNextFunction(lastStepSuccess);
                if (function == null)
                    break;
                //execute with function harness
                lastStepSuccess = await Execute(function, ctx);
            }
        }
        private async Task<bool> Execute(FunctionHarness function, CancellationToken ctx = default(CancellationToken))
        {
            Thread.Sleep(_flow.StepDelayMilliseconds);
            _logger.Debug($"Executing step for {_flow.Name}");
            try
            {
                await function.Execute(ctx);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return false;
            }
            return function.Status == "Complete";
        }
    }
}
