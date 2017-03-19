using System.Threading;
using System.Threading.Tasks;
using NLog;
using Runic.Agent.FlowManagement;
using Runic.Agent.Harness;
using Runic.Agent.Messaging;
using Runic.Framework.Models;
using Runic.Agent.AssemblyManagement;

namespace Runic.Agent.Service
{
    public class AgentService : IAgentService
    {
        private IMessagingService _messagingService { get; }
        private ExecutionContext _executionContext { get; set; }
        private CancellationToken _ct { get; set; }
        private FlowHarness _flowHarness { get; set; }
        private readonly Flows _flows;
        private readonly PluginManager _pluginManager;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();


        public AgentService(PluginManager pluginManager, Flows flows)
        {
            _executionContext = new ExecutionContext();
            _flowHarness = new FlowHarness();
            _flows = flows;
            _pluginManager = pluginManager;
        }

        private void RegisterHandlers(IMessagingService messagingService, CancellationToken ct = default(CancellationToken))
        {
            messagingService.RegisterThreadLevelHandler((message, context) => SetThreadLevel(message, ct));
            messagingService.RegisterFlowUpdateHandler((message, context) => Task.Run(() => _flows.AddUpdateFlow(message.Flow), ct));
            _logger.Debug("Registered message handlers");
        }

        public async Task Run(IMessagingService messagingService, CancellationToken ct = default(CancellationToken))
        {
            _ct = ct;
            RegisterHandlers(messagingService, ct);
            //todo use better pattern
            while (!ct.IsCancellationRequested)
            {
                await Task.Run(() => Thread.Sleep(5000), ct);
            }
        }

        public void StartFlow(FlowContext flowContext)
        {
            _logger.Debug($"Starting flow {flowContext.FlowName} at {flowContext.ThreadCount} threads");
            _executionContext.FlowContexts.Add(flowContext.FlowName, flowContext);
            flowContext.Task = _flowHarness.Execute(flowContext.Flow, flowContext.ThreadCount, _ct);
            flowContext.CancellationToken = _ct;
        }

        public int GetThreadLevel(string flow)
        {
            FlowContext context;
            if (_executionContext.FlowContexts.TryGetValue(flow, out context))
            {
                return context.ThreadCount;
            }
            return 0;
        }

        public async Task SetThreadLevel(SetThreadLevelRequest request, CancellationToken ct = default(CancellationToken))
        {
            _logger.Debug($"Attempting to update thread level to {request.ThreadLevel} for {request.FlowName}");

            if (_executionContext == null)
            {
                _executionContext = new ExecutionContext();
            }

            //if not enough threads then error
            if (!_executionContext.ThreadsAreAvailable(request.ThreadLevel, request.FlowName))
            {
                _logger.Error($"Not enough available threads.");
                throw new NotEnoughThreadsAvailableException();
            }

            if (_executionContext.FlowHarnesses.ContainsKey(request.FlowName))
            {
                _logger.Debug($"Update thread level to {request.ThreadLevel} for {request.FlowName}");
                //flow harness found - update the thread
                await _executionContext.FlowHarnesses[request.FlowName].UpdateThreads(request.ThreadLevel, ct);
            }
            else
            {
                _logger.Debug($"Starting new flow {request.FlowName} at {request.ThreadLevel} threads");
                //no harness found, start the threads
                await Task.Run(() => StartFlow(new FlowContext()
                {
                    FlowName = request.FlowName,
                    ThreadCount = request.ThreadLevel,
                    Flow = _flows.GetFlow(request.FlowName)
                }), ct);
            }
        }
    }
}
