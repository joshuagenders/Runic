using Runic.Agent.Core.Services;
using Runic.Agent.TestHarness.Services;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.ThreadManagement
{
    public class PopulationControl : IPopulationControl
    {
        private readonly IPerson _runnerService;
        private readonly IEventService _eventService;

        private static ConcurrentDictionary<string, JourneyControl> _threadManagers { get; set; }

        public PopulationControl(IPerson runnerService, IEventService eventService)
        {
            _runnerService = runnerService;
            _threadManagers = new ConcurrentDictionary<string, JourneyControl>();
            _eventService = eventService;
        }

        public int GetThreadLevel(string flowId)
        {
            if (_threadManagers.TryGetValue(flowId, out JourneyControl manager))
            {
                return manager.GetCurrentThreadCount();
            }

            return 0;
        }

        public void StopFlow(string flowId)
        {
            if (_threadManagers.TryRemove(flowId, out JourneyControl threadManager))
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

        public async Task SetThreadLevelAsync(string flowId, Framework.Models.Journey flow, int threadLevel, CancellationToken ctx = default(CancellationToken))
        {
            //TODO implement maxthreads
            _eventService.Debug($"Attempting to update thread level to {threadLevel} for {flow.Name}");
            
            if (_threadManagers.TryGetValue(flowId, out JourneyControl manager))
            {
                await manager.UpdateThreadCountAsync(threadLevel);
            }
            else
            {
                var resolvedManager = _threadManagers.GetOrAdd(flowId, new JourneyControl(flow, _runnerService, _eventService));
                await resolvedManager.UpdateThreadCountAsync(threadLevel);
            }
            _eventService.OnThreadChange(flow, threadLevel);
        }

        public IList<string> GetRunningFlows() => _threadManagers.Select(t => t.Key).ToList();
        public int GetRunningFlowCount() => _threadManagers.Count;
        public bool FlowExists(string flowId) => _threadManagers.ContainsKey(flowId);
    }
}
