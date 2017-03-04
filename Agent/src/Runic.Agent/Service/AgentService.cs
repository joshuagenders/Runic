using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        private string _subscriberId { get; }

        public AgentService(IMessagingService messagingService)
        {
            _messagingService = messagingService;
            _subscriberId = Guid.NewGuid().ToString("n");
        }

        private void RegisterHandlers(CancellationToken handlerCt)
        {
            _messagingService.RegisterThreadLevelHandler<SetThreadLevelRequest>(_subscriberId,
                (message) => SetThreadLevel(message, handlerCt));

            _messagingService.RegisterFlowUpdateHandler<AddUpdateFlowRequest>(_subscriberId,
                (message) => Task.Run(() => Flows.AddUpdateFlow(message.Flow), handlerCt));
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
            var harness = new FlowHarness();
            _executionContext.FlowContexts.Add(flowContext.FlowName, flowContext);
            flowContext.Task = harness.Execute(flowContext.Flow, new ThreadControl(flowContext.ThreadCount), _ct);
            flowContext.CancellationToken = _ct;
        }

        public async Task AddUpdateFlow(Flow flow, CancellationToken ct)
        {
            await Task.Run(() => Flows.AddUpdateFlow(flow), ct);
            //todo update running flow
        }

        public async Task SetThreadLevel(SetThreadLevelRequest request, CancellationToken ct)
        {
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
