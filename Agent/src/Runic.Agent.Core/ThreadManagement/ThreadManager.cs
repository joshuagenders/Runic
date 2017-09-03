﻿using Runic.Agent.Core.PluginManagement;
using Runic.Agent.Core.Configuration;
using Runic.Agent.Core.ExternalInterfaces;
using Runic.Agent.Core.FlowManagement;
using Runic.Agent.Core.Services;
using Runic.Framework.Clients;
using Runic.Framework.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.ThreadManagement
{
    public class ThreadManager : IThreadManager
    {
        private readonly ILoggingHandler _log;
        private readonly IStatsClient _stats;
        private readonly IPluginManager _pluginManager; 
        private readonly IFlowManager _flowManager;
        private readonly IRunnerService _runnerService;
        private readonly AgentCoreConfiguration _config;
        private readonly ITestResultHandler _testResultHandler;

        private static ConcurrentDictionary<string, FlowThreadManager> _threadManagers { get; set; }

        public ThreadManager(
            IFlowManager flowManager,
            IPluginManager pluginManager,
            IStatsClient stats,
            IRunnerService runnerService, 
            ILoggingHandler loggingHandler,
            AgentCoreConfiguration config,
            ITestResultHandler testResultHandler)
        {
            _log = loggingHandler;
            _flowManager = flowManager;
            _pluginManager = pluginManager;
            _stats = stats;
            _runnerService = runnerService;
            _threadManagers = new ConcurrentDictionary<string, FlowThreadManager>();
            _config = config;
            _testResultHandler = testResultHandler;
        }

        public int GetThreadLevel(string flowId)
        {
            if (_threadManagers.TryGetValue(flowId, out FlowThreadManager manager))
            {
                return manager.GetCurrentThreadCount();
            }

            return 0;
        }

        public void StopFlow(string flowExecutionId)
        {
            if (_threadManagers.TryRemove(flowExecutionId, out FlowThreadManager threadManager))
            {
                threadManager.StopAll();
            }
        }
        
        public async Task CancelAll(CancellationToken ctx = default(CancellationToken))
        {
            var updateTasks = new List<Task>();
            _threadManagers.ToList().ForEach(ftm => updateTasks.Add(ftm.Value.UpdateThreadCountAsync(0)));
            await Task.WhenAll(updateTasks.ToArray());
            foreach (var manager in _threadManagers)
            {
                manager.Value.StopAll();
            }
        }

        public async Task SetThreadLevelAsync(SetThreadLevelRequest request, CancellationToken ctx = default(CancellationToken))
        {
            //TODO implement maxthreads
            _log.Debug($"Attempting to update thread level to {request.ThreadLevel} for {request.FlowName}");
            
            if (_threadManagers.TryGetValue(request.FlowId, out FlowThreadManager manager))
            {
                await manager.UpdateThreadCountAsync(request.ThreadLevel);
            }
            else
            {
                var newThreadManager = _flowManager.GetFlowThreadManager(
                    request.FlowName, 
                    _pluginManager, 
                    _stats, 
                    _runnerService,
                    _log,
                    _config);

                var resolvedManager = _threadManagers.GetOrAdd(request.FlowId, newThreadManager);
                await resolvedManager.UpdateThreadCountAsync(request.ThreadLevel);
            }
            _testResultHandler.OnThreadChange(_flowManager.GetFlow(request.FlowName), request.ThreadLevel);
            _stats.SetThreadLevel(request.FlowName, request.ThreadLevel);
        }

        public IList<string> GetRunningFlows() => _threadManagers.Select(t => t.Key).ToList();
        public int GetRunningFlowCount() => _threadManagers.Count;
        public bool FlowExists(string flowExecutionId) => _threadManagers.ContainsKey(flowExecutionId);
    }
}
