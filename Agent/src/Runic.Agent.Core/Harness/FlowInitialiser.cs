﻿using Microsoft.Extensions.Logging;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Framework.Models;
using System;

namespace Runic.Agent.Core.Harness
{
    public class FlowInitialiser
    {
        private static readonly ILogger _logger = new LoggerFactory().CreateLogger<FunctionHarness>();
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
                    _logger.LogError($"Encountered error initialising flow {flow.Name}", e);
                }
            }
        }

        private void LoadLibrary(Step step)
        {
            _logger.LogDebug($"Attempting to load library for {step.Function.FunctionName} in {step.Function.AssemblyName}");
            _pluginManager.LoadPlugin(step.Function.AssemblyName);
        }
    }
}
