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
using Runic.Agent.ThreadPatterns;
using Runic.Agent.Metrics;

namespace Runic.Agent.Service
{
    public class AgentService : IAgentService
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private IMessagingService _messagingService { get; }
        private readonly IFlowManager _flowManager;
        private readonly IPluginManager _pluginManager;
        private readonly IStats _stats;

        private ConcurrentDictionary<string, ThreadManager> _flowThreadManagers { get; set; }

        public AgentService(IPluginManager pluginManager, 
                            IMessagingService messagingService, 
                            IFlowManager flowManager, 
                            IStats stats)
        {
            _flowManager = flowManager;
            _pluginManager = pluginManager;
            _messagingService = messagingService;
            _stats = stats;

            _flowThreadManagers = new ConcurrentDictionary<string, ThreadManager>();
        }

        public async Task Run(CancellationToken ct)
        {
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

        private async Task ExecutePattern(Flow flow, IThreadPattern pattern, CancellationToken ct)
        {
            _flowManager.AddUpdateFlow(flow);

            pattern.RegisterThreadChangeHandler(async (threadLevel) =>
            {
                await SetThreadLevel(new SetThreadLevelRequest()
                {
                    FlowName = flow.Name,
                    ThreadLevel = threadLevel
                }, ct);
            });
            await pattern.Start(ct);
        }

        public async Task ExecuteFlow(GradualFlowExecutionRequest request, CancellationToken ct)
        {
            //todo use IDs so multiple instances of the same test are supported
            var pattern = new GradualThreadPattern();
            await ExecutePattern(request.Flow, pattern, ct);
        }

        public async Task ExecuteFlow(GraphFlowExecutionRequest request, CancellationToken ct)
        {
            var pattern = new GraphThreadPattern()
            {
                DurationSeconds = request.ThreadPattern.DurationSeconds,
                Points = request.ThreadPattern.Points
            };
            await ExecutePattern(request.Flow, pattern, ct);
        }

        public async Task ExecuteFlow(ConstantFlowExecutionRequest request, CancellationToken ct)
        {
            var pattern = new ConstantThreadPattern();
            await ExecutePattern(request.Flow, pattern, ct);
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
                var newThreadManager = new ThreadManager(_flowManager.GetFlow(request.FlowName), _pluginManager, _stats);
                var resolvedManager = _flowThreadManagers.GetOrAdd(request.FlowName, newThreadManager);
                await resolvedManager.SafeUpdateThreadCountAsync(request.ThreadLevel);
            }
        }
    }
}
