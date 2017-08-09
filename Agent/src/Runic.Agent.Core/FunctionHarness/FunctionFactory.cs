using Microsoft.Extensions.Logging;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.ExternalInterfaces;
using Runic.Framework.Clients;
using Runic.Framework.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Runic.Agent.Core.FunctionHarness
{
    public class FunctionFactory : IFunctionFactory
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IStatsClient _stats;
        private readonly IDataService _dataService;
        private readonly IPluginManager _pluginManager;

        private Dictionary<string, object> _assemblies { get; set; }
        private Step _lastStep { get; set; }
        private int _lastStepCount { get; set; }
        
        public FunctionFactory(IPluginManager pluginManager, IStatsClient stats, IDataService dataService, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<FunctionFactory>();
            _loggerFactory = loggerFactory;
            _pluginManager = pluginManager;
            _stats = stats;
            _dataService = dataService;
        }

        //todo think about state lifecycle for functions and assemblies
        public FunctionHarness CreateFunction(Step step, TestContext testContext)
        {
            _logger.LogDebug($"Initialising {step.Function.FunctionName} in {step.Function.AssemblyName}");
            _logger.LogDebug($"Retrieving function type");
            var instance = _pluginManager.GetInstance(step.Function.AssemblyQualifiedClassName);
            
            //populate test context
            var testContextProperties = instance.GetType().GetProperties().Where(p => p.GetType() == typeof(TestContext) && p.GetType().GetTypeInfo().IsPublic);
            if (testContextProperties.Any())
            {
                foreach (var prop in testContextProperties)
                {
                    prop.SetValue(instance, testContext);
                }
            }
            var harness = new FunctionHarness(_stats, _loggerFactory);
            var methodParams = _dataService.GetMethodParameterValues(
                            step.DataInput?.InputDatasource,
                            step.DataInput?.DatasourceMapping);
            harness.Bind(instance,
                         step.StepName,
                         step.Function.FunctionName,
                         step.GetNextStepFromFunctionResult,
                         methodParams);
            return harness;
        }
    }
}
