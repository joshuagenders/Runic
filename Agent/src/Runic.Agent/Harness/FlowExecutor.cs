using NLog;
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

        public async Task ExecuteFlow(Flow flow, CancellationToken ctx = default(CancellationToken))
        {
            var navigator = new FlowNavigator(flow);
            FunctionHarness function = null;
            while (!ctx.IsCancellationRequested)
            {
                function = navigator.GetNextFunction();
                //execute with function harness
                
                Thread.Sleep(flow.StepDelayMilliseconds);
                _logger.Debug($"Executing step for {flow.Name}");
                try
                {
                    await function.Execute(ctx);
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                }
            }
        }
    }
}
