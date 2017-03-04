using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.DTO;
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
        private Dictionary<string, ITestController> _flowControllers { get; set; }

        public AgentService(IMessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        public async Task Run(IMessagingService service, CancellationToken ct)
        {
            _executionContext = new ExecutionContext();
            _flowControllers = new Dictionary<string, ITestController>();

            _messagingService.RegisterThreadLevelHandler<SetThreadLevelRequest>(Guid.NewGuid().ToString("n"), 
                (message) => SetThreadLevel(message, ct));

            _messagingService.RegisterFlowUpdateHandler<AddUpdateFlowRequest>(Guid.NewGuid().ToString("n"),
                (message) => Task.Run(() => Flows.AddUpdateFlow(message.Flow), ct));

            while (!ct.IsCancellationRequested)
            {
                foreach (var flowContext in _executionContext.FlowContexts)
                {
                    if (_flowControllers.ContainsKey(flowContext.Key))
                    {

                    }
                    else
                    {
                        
                    }
                }
            }
        }

        private void CreateFlowController(FlowContext flowContext)
        {
            lock (_flowControllers)
            {
                _flowControllers.Add(flowContext.FlowName, new FlowController(flowContext.ThreadCount));
            }
        }

        public Task AddUpdateFlow(Flow flow, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task SetThreadLevel(SetThreadLevelRequest request, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
