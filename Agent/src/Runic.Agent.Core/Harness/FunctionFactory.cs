using Microsoft.Extensions.Logging;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Data;
using Runic.Agent.Core.Metrics;
using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Runic.Agent.Core.Harness
{
    public class FunctionFactory
    {
        private static readonly ILogger _logger = new LoggerFactory().CreateLogger(nameof(FunctionFactory));
        private readonly Flow _flow;
        private readonly IStats _stats;
        private readonly IDataService _dataService;
        private readonly IPluginManager _pluginManager;

        private Dictionary<string, object> _assemblies { get; set; }
        private Step _lastStep { get; set; }
        private int _lastStepCount { get; set; }
        
        public FunctionFactory(Flow flow, IPluginManager pluginManager, IStats stats, IDataService dataService)
        {
            _flow = flow;
            _pluginManager = pluginManager;
            _stats = stats;
            _dataService = dataService;
        }
        
        public FunctionHarness CreateFunction(string stepName)
        { 
            return CreateFunction(GetStepByName(stepName));
        }

        private Step GetStepByName(string name)
        {
            //if no next step, restart
            if (string.IsNullOrWhiteSpace(name))
            {
                return _flow.Steps[0];
            }
            return _flow.Steps.Where(s => s.StepName == name).Single();
        }

        public FunctionHarness CreateFunction(Step step)
        {
            _logger.LogDebug($"Initialising {step.Function.FunctionName} in {step.Function.AssemblyName}");
            _logger.LogDebug($"Retrieving function type");

            var type = _pluginManager.GetClassType(step.Function.AssemblyQualifiedClassName);
            if (type == null)
                throw new FunctionTypeNotFoundException();

            _logger.LogDebug($"type found {type.AssemblyQualifiedName}");
            var instance = Activator.CreateInstance(type);
            _logger.LogDebug($"{step.Function.FunctionName} in {step.Function.AssemblyName} initialised");

            var harness = new FunctionHarness(_stats);
            harness.Bind(instance,
                         step.StepName,
                         step.Function.FunctionName,
                         step.GetNextStepFromFunctionResult,
                         _dataService.GetMethodParameterValues(
                            step.DataInput.InputDatasource, 
                            step.DataInput.DatasourceMapping));
            return harness;
        }
    }
}
