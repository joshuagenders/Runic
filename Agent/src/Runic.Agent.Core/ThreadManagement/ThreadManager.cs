using NLog;
using Runic.Framework.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.ThreadManagement
{
    public class ThreadManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static ConcurrentDictionary<string, FlowThreadManager> _threadManagers { get; set; }
        private readonly ExecutionContext _context;
        public ThreadManager(ExecutionContext context)
        {
            _threadManagers = new ConcurrentDictionary<string, FlowThreadManager>();
            _context = context;
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
        
        public async Task SafeCancelAll()
        {
            var updateTasks = new List<Task>();
            _threadManagers.ToList().ForEach(ftm => updateTasks.Add(ftm.Value.SafeUpdateThreadCountAsync(0)));
            await Task.WhenAll(updateTasks.ToArray());
            foreach (var manager in _threadManagers)
            {
                manager.Value.StopAll();
            }
        }

        public async Task SetThreadLevelAsync(SetThreadLevelRequest request, CancellationToken ct)
        {
            //todo implement maxthreads
            _logger.Debug($"Attempting to update thread level to {request.ThreadLevel} for {request.FlowName}");
            if (_threadManagers.TryGetValue(request.FlowId, out FlowThreadManager manager))
            {
                await manager.SafeUpdateThreadCountAsync(request.ThreadLevel);
            }
            else
            {
                var newThreadManager = request.FlowName
                                              .GetFlowThreadManager(
                                                    _context.flowManager, 
                                                    _context.pluginManager, 
                                                    _context.stats, 
                                                    _context.dataService);

                var resolvedManager = _threadManagers.GetOrAdd(request.FlowId, newThreadManager);
                await resolvedManager.SafeUpdateThreadCountAsync(request.ThreadLevel);
            }
        }

        public IList<string> GetRunningFlows => _threadManagers.Select(t => t.Key).ToList();
        public int GetRunningFlowCount => _threadManagers.Count;
        public bool FlowExists(string flowExecutionId) => _threadManagers.ContainsKey(flowExecutionId);
    }
}
