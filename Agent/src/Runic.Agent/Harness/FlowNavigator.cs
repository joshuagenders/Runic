using NLog;
using Runic.Agent.AssemblyManagement;
using Runic.Framework.Models;
using System;
using System.Collections.Generic;

namespace Runic.Agent.Harness
{
    public class FlowNavigator
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private Flow _flow { get; set; }
        private Dictionary<string, object> _assemblies { get; set; }
        private int _lastStep { get; set; }

        public FlowNavigator(Flow flow)
        {
            _flow = flow;
            _lastStep = 0;
        }

        public FunctionHarness GetNextFunction()
        {
            return CreateFunction(GetNextStep());
        }

        private Step GetNextStep()
        {
            _lastStep++;
            if (_flow.Steps.Count > _lastStep)
            {
                _lastStep = 0;
            }
            var step = _flow.Steps[_lastStep];
            return step;
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
