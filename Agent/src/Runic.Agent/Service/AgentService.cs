using System.Threading;
using System.Threading.Tasks;
using NLog;
using Runic.Agent.FlowManagement;
using Runic.Agent.Harness;
using Runic.Agent.Messaging;
using Runic.Framework.Models;
using Runic.Agent.AssemblyManagement;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;

namespace Runic.Agent.Service
{
    public class AgentService : IAgentService
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private IMessagingService _messagingService { get; }
        private readonly Flows _flows;
        private readonly PluginManager _pluginManager;

        private ConcurrentDictionary<string, ThreadManager> _flowThreadManagers { get; set; }

        public AgentService(PluginManager pluginManager, Flows flows)
        {
            _flows = flows;
            _pluginManager = pluginManager;
            _flowThreadManagers = new ConcurrentDictionary<string, ThreadManager>();
        }

        private void RegisterHandlers(IMessagingService messagingService, CancellationToken ct)
        {
            messagingService.RegisterThreadLevelHandler((message, context) => SetThreadLevel(message, ct));
            messagingService.RegisterFlowUpdateHandler((message, context) => Task.Run(() => _flows.AddUpdateFlow(message.Flow), ct));
            _logger.Debug("Registered message handlers");
        }

        public async Task Run(IMessagingService messagingService, CancellationToken ct)
        {
            RegisterHandlers(messagingService, ct);

            //wait for cancellation
            await Task.Run(() => 
            {
                var mre = new ManualResetEventSlim(false);
                ct.Register(() => mre.Set());
                mre.Wait();
            }, ct);
            SafeCancelAll();
        }

        public void SafeCancelAll()
        {
            List<Task> updateTasks = new List<Task>();
            lock (_flowThreadManagers)
            {
                _flowThreadManagers.ToList().ForEach(ftm => updateTasks.Add(ftm.Value.SafeUpdateThreadCountAsync(0)));
            }
            Task.WaitAll(updateTasks.ToArray());
        }

        public int GetThreadLevel(string flow)
        {
            if (_flowThreadManagers.TryGetValue(flow, out ThreadManager manager))
            {
                return manager.GetCurrentThreadCount();
            }

            return 0;
        }

        public async Task SetThreadLevel(SetThreadLevelRequest request, CancellationToken ct)
        {
            //todo implement maxthreads
            _logger.Debug($"Attempting to update thread level to {request.ThreadLevel} for {request.FlowName}");
            if (_flowThreadManagers.TryGetValue(request.FlowName, out ThreadManager manager))
            {
                await manager.SafeUpdateThreadCountAsync(request.ThreadLevel);
            }
            else
            {
                var newThreadManager = new ThreadManager(_flows.GetFlow(request.FlowName), _pluginManager);
                var resolvedManager = _flowThreadManagers.GetOrAdd(request.FlowName, newThreadManager);
                await resolvedManager.SafeUpdateThreadCountAsync(request.ThreadLevel);
            }
        }
    }
}
