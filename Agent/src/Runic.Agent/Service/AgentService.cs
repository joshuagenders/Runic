using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using NLog;
using Runic.Agent.FlowManagement;
using Runic.Agent.Harness;
using Runic.Agent.Messaging;
using Runic.Core.Models;

namespace Runic.Agent.Service
{
    public class AgentService : IAgentService
    {
        private IMessagingService _messagingService { get; }
        private ExecutionContext _executionContext { get; set; }
        private CancellationToken _ct { get; set; }
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();


        public AgentService(IMessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        private void RegisterHandlers(CancellationToken handlerCt)
        {
            _messagingService.RegisterThreadLevelHandler((message, context) => SetThreadLevel(message, handlerCt));
            _messagingService.RegisterFlowUpdateHandler((message, context) => Task.Run(() => Flows.AddUpdateFlow(message.Flow), handlerCt));
            _logger.Info("Registered message handlers");
        }

        public async Task Run(IMessagingService service, CancellationToken ct)
        {
            _executionContext = new ExecutionContext();
            _ct = ct;

            RegisterHandlers(ct);

            while (!ct.IsCancellationRequested)
            {
                await Task.Run(() => Thread.Sleep(5000), ct);
            }
        }

        private void StartFlow(FlowContext flowContext)
        {
            _logger.Info($"Starting flow {flowContext.FlowName} at {flowContext.ThreadCount} threads");
            var harness = Program.Container.Resolve<IFlowHarness>();
            _executionContext.FlowContexts.Add(flowContext.FlowName, flowContext);
            flowContext.Task = harness.Execute(flowContext.Flow, new ThreadControl(flowContext.ThreadCount), _ct);
            flowContext.CancellationToken = _ct;
        }

        public async Task AddUpdateFlow(Flow flow, CancellationToken ct)
        {
            _logger.Info($"Updating {flow.Name}");
            await Task.Run(() => Flows.AddUpdateFlow(flow), ct);
            //todo update running flow
        }

        public async Task SetThreadLevel(SetThreadLevelRequest request, CancellationToken ct)
        {
            _logger.Info($"Setting thread level to {request.ThreadLevel} for {request.FlowName}");
            //if not enough threads then error
            if (!_executionContext.ThreadsAreAvailable(request.ThreadLevel, request.FlowName))
                throw new NotEnoughThreadsAvailableException();

            if (_executionContext.FlowHarnesses.ContainsKey(request.FlowName))
            {
                //flow harness found - update the thread
                _executionContext.FlowHarnesses[request.FlowName].UpdateThreads(request.ThreadLevel);
            }
            else
            {
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
