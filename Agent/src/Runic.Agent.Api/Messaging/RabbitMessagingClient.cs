using RawRabbit;
using RawRabbit.Configuration;
using RawRabbit.vNext;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Api.Messaging
{
    public class RabbitMessagingClient
    {
        private IBusClient _bus { get; }

        public RabbitMessagingClient()
        {
            //todo config
            var busConfig = new RawRabbitConfiguration
            {
                Username = "guest",
                Password = "guest",
                Port = 5672,
                VirtualHost = "/",
                Hostnames = { "localhost" }
            };
            _bus = BusClientFactory.CreateDefault(busConfig);
        }
        
        public async Task PublishMessageAsync<T>(T message, CancellationToken ct = default(CancellationToken))
        {
            await _bus.PublishAsync(message);
        }
    }
}
