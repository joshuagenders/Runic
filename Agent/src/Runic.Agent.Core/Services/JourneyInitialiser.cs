using Runic.Agent.Core.PluginManagement;
using Runic.Agent.Core.Services;
using Runic.Agent.Framework.Models;
using System;

namespace Runic.Agent.Core.FlowManagement
{
    public class JourneyInitialiser
    {
        private readonly IPluginManager _pluginManager;
        private readonly IEventService _eventService;

        public JourneyInitialiser(IPluginManager pluginManager, IEventService eventService)
        {
            _pluginManager = pluginManager;
            _eventService = eventService;
        }

        public void InitialiseJourney(Journey journey)
        {
            foreach (var step in journey.Steps)
            {
                try
                {
                    //load the library if needed
                    LoadLibrary(step);
                }
                catch (Exception ex)
                {
                    _eventService.Error($"Encountered error initialising journey {journey.Name}", ex);
                    throw;
                }
            }
        }

        private void LoadLibrary(Step step)
        {
            if (step.Function != null)
            {
                _eventService.Debug($"Attempting to load library for {step.Function.FunctionName} in {step.Function.AssemblyName}");
                _pluginManager.LoadPlugin(step.Function.AssemblyName);
            }
            else if (step.Cucumber != null)
            {
                _eventService.Debug($"Attempting to load library for cucumber in {step.Cucumber.AssemblyName}");
                _pluginManager.LoadPlugin(step.Cucumber.AssemblyName);
            }
        }
    }
}
