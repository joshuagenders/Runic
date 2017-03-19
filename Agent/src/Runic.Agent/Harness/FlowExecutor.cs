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
                //execute with function harness
                
                Thread.Sleep(_flow.StepDelayMilliseconds);
                _logger.Debug($"Executing step for {_flow.Name}");
                try
                {
                    await function.Execute(ctx);
                    if (function.Status == "Complete")
                    {  
                        lastStepSuccess = true;
                    }
                    else
                    { 
                        lastStepSuccess = false;
                    }
                }
                catch (Exception e)
                {
                    lastStepSuccess = false;
                    _logger.Error(e);
                }
            }
        }
    }
}
