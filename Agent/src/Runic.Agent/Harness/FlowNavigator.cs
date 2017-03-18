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
        private string _lastStep { get; set; }

        public FlowNavigator(Flow flow)
        {
            _flow = flow;
            _lastStep = String.Empty;
        }

        public FunctionHarness GetNextFunction(bool lastStepSuccess)
        {
            return CreateFunction(GetNextStep(lastStepSuccess));
        }

        private Step GetNextStep(bool lastStepResultSuccess)
        {
            if (string.IsNullOrEmpty(_lastStep))
                return _flow.Steps[0];

            var lastStep = _flow.Steps.Where(s => s.StepName == _lastStep).Single();
            if (!lastStepResultSuccess)
            {
                return _flow.Steps.Where(s => s.StepName == lastStep.NextStepOnFailure).Single();
            }
            return _flow.Steps.Where(s => s.StepName == lastStep.NextStepOnSuccess).Single();
        }

        public FunctionHarness CreateFunction(Step step)
        {
            _logger.Debug($"Initialising {step.Function.FunctionName} in {step.Function.AssemblyName}");
            _logger.Debug($"Retrieving function type");

            var type = PluginManager.GetFunctionType(step.Function.FunctionName);
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
