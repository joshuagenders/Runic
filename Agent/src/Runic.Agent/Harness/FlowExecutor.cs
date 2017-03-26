using NLog;
using Runic.Agent.AssemblyManagement;
using Runic.Agent.Metrics;
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
        public async Task ExecuteFlow(CancellationToken ct)
        {
            var navigator = new FlowNavigator(_flow, _pluginManager);
            FunctionHarness function = null;
            bool lastStepSuccess = false;
            while (!ct.IsCancellationRequested)
            {
                function = navigator.GetNextFunction(lastStepSuccess);
                lastStepSuccess = await function.Execute(ct);
            }
        }
    }
}
