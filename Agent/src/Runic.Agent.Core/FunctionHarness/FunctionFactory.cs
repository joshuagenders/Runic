using Runic.Agent.Core.PluginManagement;
using Runic.Agent.Core.Services;
using Runic.Framework.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Runic.Agent.Core.FunctionHarness
{
    public class FunctionFactory : IFunctionFactory
    {
        private readonly IDataService _dataService;
        private readonly IPluginManager _pluginManager;
        private readonly IEventService _eventService;

        public FunctionFactory(IPluginManager pluginManager, IDataService dataService, IEventService eventService)
        {
            _eventService = eventService;
            _pluginManager = pluginManager;
            _dataService = dataService;
        }

        //todo think about state lifecycle for functions and assemblies
        public FunctionHarness CreateFunction(Step step, TestContext testContext)
        {
            _eventService.Debug($"Initialising {step.Function.FunctionName} in {step.Function.AssemblyName}");
            _eventService.Debug($"Retrieving function type");
            var instance = _pluginManager.GetInstance(step.Function.AssemblyQualifiedClassName);

            var testContextProperties = instance.GetType()
                                                .GetProperties()
                                                .Where(p => p.GetType() == typeof(TestContext) 
                                                         && p.GetType().GetTypeInfo().IsPublic);

            if (testContextProperties.Any())
            {
                foreach (var prop in testContextProperties)
                {
                    prop.SetValue(instance, testContext);
                }
            }

            var harness = new FunctionHarness(_eventService, _dataService);
            harness.Bind(instance, step);
            return harness;
        }
    }
}
