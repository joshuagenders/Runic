using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Runic.Agent.Configuration;
using Runic.Agent.DTO;
using Runic.Core.Models;

namespace Runic.Agent.Messaging
{
    public class RabbitMessagingService : IMessagingService
    {
        private static readonly IBus _bus = RabbitHutch.CreateBus(AgentConfiguration.ClientConnectionConfiguration);

        public void RegisterThreadLevelHandler<T>(string subscriptionId, Func<SetThreadLevelRequest, Task> handler)
        {
            _bus.SubscribeAsync(subscriptionId, handler);
        }

        public void RegisterFlowUpdateHandler<T>(string subscriptionId, Func<AddUpdateFlowRequest, Task> handler)
        {
            _bus.SubscribeAsync(subscriptionId, handler);
        }

        public async Task<SetThreadLevelRequest> ReceiveThreadLevelRequest(CancellationToken ct)
        {
            SetThreadLevelRequest request = null;
            await Task.Run(() => _bus.Receive<SetThreadLevelRequest>(typeof(SetThreadLevelRequest).Name, result => { request = result; }), ct);
            return request;
        }

        public async Task<AddUpdateFlowRequest> ReceiveUpdateFlowRequest(CancellationToken ct)
        {
            AddUpdateFlowRequest request = null;
            await Task.Run(() => _bus.Receive<AddUpdateFlowRequest>(typeof(AddUpdateFlowRequest).Name, result => { request = result; }), ct);
            return request;
        }
    }
}
