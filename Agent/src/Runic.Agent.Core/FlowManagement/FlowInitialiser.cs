using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.ExternalInterfaces;
using Runic.Framework.Models;
using System;

namespace Runic.Agent.Core.FlowManagement
{
    public class FlowInitialiser
    {
        private readonly ILoggingHandler _log;
        private readonly IPluginManager _pluginManager;

        public FlowInitialiser(IPluginManager pluginManager, ILoggingHandler loggingHandler)
        {
            _pluginManager = pluginManager;
            _log = loggingHandler;
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
                catch (Exception ex)
                {
                    _log.Error($"Encountered error initialising flow {flow.Name}", ex);
                    throw ex;
                }
            }
        }

        private void LoadLibrary(Step step)
        {
            _log.Debug($"Attempting to load library for {step.Function.FunctionName} in {step.Function.AssemblyName}");
            _pluginManager.LoadPlugin(step.Function.AssemblyName);
        }
    }
}
