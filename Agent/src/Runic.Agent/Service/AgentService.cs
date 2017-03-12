using System.Threading;
using System.Threading.Tasks;
using Autofac;
using NLog;
using Runic.Agent.FlowManagement;
using Runic.Agent.Harness;
using Runic.Agent.Messaging;
using Runic.Framework.Models;

namespace Runic.Agent.Service
{
    public class AgentService : IAgentService
    {
        private IMessagingService _messagingService { get; }
        private ExecutionContext _executionContext { get; set; }
        private CancellationToken _ct { get; set; }
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public AgentService()
        {
            _executionContext = new ExecutionContext();
        }

        private void RegisterHandlers(IMessagingService messagingService = null, CancellationToken ct = default(CancellationToken))
        {
            if (messagingService == null)
                messagingService = IoC.Container.Resolve<IMessagingService>();
            messagingService.RegisterThreadLevelHandler((message, context) => SetThreadLevel(message, ct));
            messagingService.RegisterFlowUpdateHandler((message, context) => Task.Run(() => Flows.AddUpdateFlow(message.Flow), ct));
            _logger.Debug("Registered message handlers");
        }

        public async Task Run(IMessagingService messagingService, CancellationToken ct = default(CancellationToken))
        {
            _ct = ct;
            RegisterHandlers(messagingService, ct);

            while (!ct.IsCancellationRequested)
            {
                await Task.Run(() => Thread.Sleep(5000), ct);
            }
        }

        public void StartFlow(FlowContext flowContext)
        {
            _logger.Debug($"Starting flow {flowContext.FlowName} at {flowContext.ThreadCount} threads");
            var harness = IoC.Container.Resolve<IFlowHarness>();
            _executionContext.FlowContexts.Add(flowContext.FlowName, flowContext);
            flowContext.Task = harness.Execute(flowContext.Flow, flowContext.ThreadCount, _ct);
            flowContext.CancellationToken = _ct;
        }

        public int? GetThreadLevel(string flow)
        {
            FlowContext context;
            if (_executionContext.FlowContexts.TryGetValue(flow, out context))
            {
                return context.ThreadCount;
            }
            return null;
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
                    Flow = Flows.GetFlow(request.FlowName)
                }), ct);
            }
        }
    }
}
