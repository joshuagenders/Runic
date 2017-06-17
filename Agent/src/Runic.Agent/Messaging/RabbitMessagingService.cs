using System;
using System.Threading.Tasks;
using NLog;
using RawRabbit;
using RawRabbit.Context;
using RawRabbit.Configuration;
using RawRabbit.vNext;
using System.Threading;

namespace Runic.Agent.Services
{
    public class RabbitMessagingService : IMessagingService
    {
        private IBusClient _bus { get; }
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public RabbitMessagingService()
        {
            //todo, DI handlers
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

        public void RegisterMessageHandler<T>(Func<T, Task> handler)
        {
            Func<T, MessageContext, Task> rabbitHandler =
                (flowRequest, messageContext) => handler(flowRequest);
            
            try
            {
                _bus.SubscribeAsync(rabbitHandler);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        public async Task RunServiceAsync(CancellationToken ct)
        {
            await Task.Run(() =>
            {
                var mre = new ManualResetEventSlim(false);
                ct.Register(() => mre.Set());
                mre.Wait();
            }, ct);
            await _bus.ShutdownAsync();
        }
    }
}
