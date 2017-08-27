using Runic.Agent.Core.PluginManagement;
using Runic.Agent.Core.ExternalInterfaces;
using Runic.Framework.Models;
using System;

namespace Runic.Agent.Core.FlowManagement
{
    public class FlowInitialiser
    {
        private readonly ILoggingHandler _log;
        private readonly IPluginManager _pluginManager;
        private readonly IFlowManager _flowManager;

        public FlowInitialiser(IPluginManager pluginManager, IFlowManager flowManager, ILoggingHandler loggingHandler)
        {
            _pluginManager = pluginManager;
            _flowManager = flowManager;
            _log = loggingHandler;
        }

        public void InitialiseFlow(Flow flow)
        {
            _flowManager.AddUpdateFlow(flow);
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
            if (step.Function != null)
            {
                _log.Debug($"Attempting to load library for {step.Function.FunctionName} in {step.Function.AssemblyName}");
                _pluginManager.LoadPlugin(step.Function.AssemblyName);
            }
            else if (step.Cucumber != null)
            {
                _log.Debug($"Attempting to load library for cucumber in {step.Cucumber.AssemblyName}");
                _pluginManager.LoadPlugin(step.Cucumber.AssemblyName);
            }
        }
    }
}
