using System;
using System.Threading.Tasks;
using Autofac;
using NLog;
using RawRabbit;
using RawRabbit.Context;
using Runic.Core.Models;

namespace Runic.Agent.Messaging
{
    public class RabbitMessagingService : IMessagingService
    {
        private static readonly IBusClient _bus = IoC.Container.Resolve<IBusClient>();
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

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
