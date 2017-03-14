using System;
using System.Threading.Tasks;
using NLog;
using RawRabbit;
using RawRabbit.Context;
using Runic.Framework.Models;
using RawRabbit.Configuration;
using RawRabbit.vNext;

namespace Runic.Agent.Messaging
{
    public class RabbitMessagingService : IMessagingService
    {
        private IBusClient _bus { get; }
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public RabbitMessagingService()
        {
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

        public void RegisterThreadLevelHandler(Func<SetThreadLevelRequest, MessageContext, Task> handler)
        {
            try
            {
                _bus.SubscribeAsync(handler);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        public void RegisterFlowUpdateHandler(Func<AddUpdateFlowRequest, MessageContext, Task> handler)
        {
            try
            {
                _bus.SubscribeAsync(handler);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }
    }
}
