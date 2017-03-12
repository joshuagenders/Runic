using System;
using System.Threading.Tasks;
using Autofac;
using NLog;
using RawRabbit;
using RawRabbit.Context;
using Runic.Framework.Models;

namespace Runic.Agent.Messaging
{
    public class RabbitMessagingService : IMessagingService
    {
        private IBusClient _bus { get; }
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public RabbitMessagingService()
        {
            _bus = IoC.Container.Resolve<IBusClient>();
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
