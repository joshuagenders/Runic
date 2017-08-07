﻿using Microsoft.Extensions.Logging;
using Runic.Agent.Core.AssemblyManagement;
using Runic.Agent.Core.Data;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.Harness;
using Runic.Agent.Core.Services;
using Runic.Framework.Clients;
using Runic.Framework.Models;

namespace Runic.Agent.Core.ThreadManagement
{
    public static class FlowExtensions
    {
        public static FlowThreadManager GetFlowThreadManager(
            this IFlowManager flowManager,
            string flow,
            IPluginManager pluginManager,
            IStatsClient stats, 
            IDataService dataService,
            ILoggerFactory loggerFactory,
            IDatetimeService datetimeService)
        {
            var flowInstance = flowManager.GetFlow(flow);
            return new FlowThreadManager(
                flowInstance, 
                stats, 
                new FunctionFactory(pluginManager, stats, dataService, loggerFactory),
                new CucumberHarness(pluginManager), loggerFactory, datetimeService);
        }
    }
}
