using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Runic.Agent.Configuration;
using Runic.Core.Models;

namespace Runic.Agent.Messaging
{
    public class RabbitMessagingService : IMessagingService
    {
        private static readonly IBus _bus = RabbitHutch.CreateBus(AgentConfiguration.ClientConnectionConfiguration);

        public async Task<ExecutionRequest> ReceiveRequest(CancellationToken ct)
        {
            ExecutionRequest request = null;
            await Task.Run(() => _bus.Receive<ExecutionRequest>(typeof(ExecutionRequest).Name, result => { request = result; }), ct);
            return request;
        }
    }
}
