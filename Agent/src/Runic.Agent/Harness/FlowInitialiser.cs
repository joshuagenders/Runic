using NLog;
using Runic.Agent.AssemblyManagement;
using Runic.Framework.Models;
using System;

namespace Runic.Agent.Harness
{
    public class FlowInitialiser
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPluginManager _pluginManager;

        public FlowInitialiser(IPluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }

        public void InitialiseFlow(Flow flow)
        {
            foreach (var step in flow.Steps)
            {
                try
                {
                    //load the library if needed
                    LoadLibrary(step);
                }
                catch (Exception e)
                {
                    _logger.Error($"Encountered error initialising flow {flow.Name}");
                    _logger.Error(e);
                }
            }
        }

        private void LoadLibrary(Step step)
        {
            _logger.Debug($"Attempting to load library for {step.Function.FunctionName} in {step.Function.AssemblyName}");
            _pluginManager.LoadPlugin(step.Function.AssemblyName);
        }
    }
}
