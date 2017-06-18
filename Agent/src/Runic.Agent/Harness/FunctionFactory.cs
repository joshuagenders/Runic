using NLog;
using Runic.Agent.AssemblyManagement;
using Runic.Agent.Data;
using Runic.Agent.Metrics;
using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Runic.Agent.Harness
{
    public class FunctionFactory
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
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
            _logger.Debug($"Initialising {step.Function.FunctionName} in {step.Function.AssemblyName}");
            _logger.Debug($"Retrieving function type");

            var type = _pluginManager.GetClassType(step.Function.AssemblyQualifiedClassName);
            if (type == null)
                throw new FunctionTypeNotFoundException();

            _logger.Debug($"type found {type.AssemblyQualifiedName}");
            var instance = Activator.CreateInstance(type);
            _logger.Debug($"{step.Function.FunctionName} in {step.Function.AssemblyName} initialised");

            var harness = new FunctionHarness(_stats);
            harness.Bind(instance,
                         step.StepName,
                         step.Function.FunctionName,
                         step.GetNextStepFromFunctionResult,
                         _dataService.GetParameters(step.DataInput.InputDatasource, 
                                                    step.DataInput.DatasourceMapping));
            return harness;
        }
    }
}
