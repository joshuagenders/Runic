using NLog;
using Runic.Agent.AssemblyManagement;
using Runic.Framework.Models;
using System;

namespace Runic.Agent.Harness
{
    public class FlowInitialiser
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void InitialiseFlow(Flow flow, IPluginProvider provider)
        {
            foreach (var step in flow.Steps)
            {
                try
                {
                    //load the library if needed
                    LoadLibrary(step, provider);
                }
                catch (Exception e)
                {
                    _logger.Error($"Encountered error initialising flow {flow.Name}");
                    _logger.Error(e);
                }
            }
        }

        private void LoadLibrary(Step step, IPluginProvider provider)
        {
            _logger.Debug($"Attempting to load library for {step.Function.FunctionName} in {step.Function.AssemblyName}");
            PluginManager.LoadPlugin(step.Function.AssemblyName, provider);
        }
    }
}
