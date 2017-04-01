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

        private Flow _flow { get; set; }
        private Dictionary<string, object> _assemblies { get; set; }
        private Step _lastStep { get; set; }
        private int _lastStepCount { get; set; }
        private IStats _stats { get; set; }
        private readonly IDataService _dataService;
        private readonly IPluginManager _pluginManager;

        public FunctionFactory(Flow flow, IPluginManager pluginManager, IStats stats, IDataService dataService)
        {
            _flow = flow;
            _pluginManager = pluginManager;
            _stats = stats;
            _dataService = dataService;
        }

        public FunctionHarness GetNextFunction(bool lastStepSuccess)
        {
            return CreateFunction(GetNextStep(lastStepSuccess));
        }

        private Step GetNextStep(bool lastStepResultSuccess)
        {
            if (_lastStep == null)
                return _flow.Steps[0];

            //if repeat is not set or repeat count is met, return next step based on success
            if (_lastStep.Repeat == 0 && _lastStepCount >= _lastStep.Repeat)
            {
                _lastStepCount = 0;
                _lastStep = lastStepResultSuccess
                    ? GetStepByName(_lastStep.NextStepOnSuccess)
                    : GetStepByName(_lastStep.NextStepOnFailure);
            }

            _lastStepCount++;
            return _lastStep;
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
                         step.Function.FunctionName, 
                         _dataService.GetParameters(step.DataInput.InputDatasource, 
                                                    step.DataInput.DatasourceMapping));
            return harness;
        }
    }
}
