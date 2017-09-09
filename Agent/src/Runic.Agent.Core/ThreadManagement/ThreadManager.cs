using Runic.Agent.Core.Services;
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
        private readonly IRunnerService _runnerService;
        private readonly IEventService _eventService;

        private static ConcurrentDictionary<string, FlowThreadManager> _threadManagers { get; set; }

        public ThreadManager(IRunnerService runnerService, IEventService eventService)
        {
            _runnerService = runnerService;
            _threadManagers = new ConcurrentDictionary<string, FlowThreadManager>();
            _eventService = eventService;
        }

        public int GetThreadLevel(string flowId)
        {
            if (_threadManagers.TryGetValue(flowId, out FlowThreadManager manager))
            {
                return manager.GetCurrentThreadCount();
            }

            return 0;
        }

        public void StopFlow(string flowId)
        {
            if (_threadManagers.TryRemove(flowId, out FlowThreadManager threadManager))
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

        public async Task SetThreadLevelAsync(string flowId, Flow flow, int threadLevel, CancellationToken ctx = default(CancellationToken))
        {
            //TODO implement maxthreads
            _eventService.Debug($"Attempting to update thread level to {threadLevel} for {flow.Name}");
            
            if (_threadManagers.TryGetValue(flowId, out FlowThreadManager manager))
            {
                await manager.UpdateThreadCountAsync(threadLevel);
            }
            else
            {
                var resolvedManager = _threadManagers.GetOrAdd(flowId, new FlowThreadManager(flow, _runnerService, _eventService));
                await resolvedManager.UpdateThreadCountAsync(threadLevel);
            }
            _eventService.OnThreadChange(flow, threadLevel);
        }

        public IList<string> GetRunningFlows() => _threadManagers.Select(t => t.Key).ToList();
        public int GetRunningFlowCount() => _threadManagers.Count;
        public bool FlowExists(string flowId) => _threadManagers.ContainsKey(flowId);
    }
}
