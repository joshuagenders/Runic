using System;
using System.Threading.Tasks;
using NLog;
using RawRabbit;
using RawRabbit.Context;
using RawRabbit.Configuration;
using RawRabbit.vNext;
using System.Threading;
using Runic.Agent.Messaging;

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

        public void RegisterMessageHandler<T>(Action<T> handler) where T : class
        {
            Func<T, MessageContext, Task> rabbitHandler =
                (request, messageContext) => Task.Run(() => handler(request));
            
            try
            {
                _bus.SubscribeAsync(rabbitHandler);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        public void PublishMessage<T>(T message)
        {
            _bus.PublishAsync(message);
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
