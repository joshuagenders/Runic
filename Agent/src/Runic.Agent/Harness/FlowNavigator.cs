using NLog;
using Runic.Agent.AssemblyManagement;
using Runic.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Runic.Agent.Harness
{
    public class FlowNavigator
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private Flow _flow { get; set; }
        private Dictionary<string, object> _assemblies { get; set; }
        private Step _lastStep { get; set; }
        private int _lastStepCount { get; set; }

        private readonly PluginManager _pluginManager;

        public FlowNavigator(Flow flow, PluginManager pluginManager)
        {
            _flow = flow;
            _pluginManager = pluginManager;
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

            var harness = new FunctionHarness();
            harness.Bind(instance, step.Function.FunctionName);
            return harness;
        }
    }
}
