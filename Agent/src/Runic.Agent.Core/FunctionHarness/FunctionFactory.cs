using Runic.Agent.Core.PluginManagement;
using Runic.Agent.Core.ExternalInterfaces;
using Runic.Framework.Clients;
using Runic.Framework.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

namespace Runic.Agent.Core.FunctionHarness
{
    public class FunctionFactory : IFunctionFactory
    {
        private readonly ILoggingHandler _log;
        private readonly IStatsClient _stats;
        private readonly IDataService _dataService;
        private readonly IPluginManager _pluginManager;

        private Dictionary<string, object> _assemblies { get; set; }
        private Step _lastStep { get; set; }
        private int _lastStepCount { get; set; }
        
        public FunctionFactory(IPluginManager pluginManager, IStatsClient stats, IDataService dataService, ILoggingHandler loggingHandler)
        {
            _log = loggingHandler;
            _pluginManager = pluginManager;
            _stats = stats;
            _dataService = dataService;
        }

        //todo think about state lifecycle for functions and assemblies
        public FunctionHarness CreateFunction(Step step, TestContext testContext)
        {
            _log.Debug($"Initialising {step.Function.FunctionName} in {step.Function.AssemblyName}");
            _log.Debug($"Retrieving function type");
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
            var harness = new FunctionHarness(_stats, _log);
            //todo think about how parameters should be passed
            //var methodParams = _dataService.GetMethodParameterValues(
            //                step.DataInput?.InputDatasource,
            //                step.DataInput?.DatasourceMapping);
            var methodParams = GetParams(step.Function.Parameters);
            harness.Bind(instance,
                         step.StepName,
                         step.Function.FunctionName,
                         step.GetNextStepFromFunctionResult,
                         methodParams);
            return harness;
        }

        private object[] GetParams(Dictionary<string,Type> parameters)
        {
            if (parameters == null || !parameters.Any())
            {
                return new object[] { };
            }
            return parameters.Select(p => Convert.ChangeType(p.Key, p.Value)).ToArray();
        }
    }
}
