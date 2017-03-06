using System;
using System.Threading.Tasks;
using Autofac;
using RawRabbit;
using RawRabbit.Context;
using Runic.Core.Models;

namespace Runic.Agent.Messaging
{
    public class RabbitMessagingService : IMessagingService
    {
        private static readonly IBusClient _bus = IoC.Container.Resolve<IBusClient>();

        public void RegisterThreadLevelHandler(Func<SetThreadLevelRequest, MessageContext, Task> handler)
        {
            _bus.SubscribeAsync(handler);
        }

        public void RegisterFlowUpdateHandler(Func<AddUpdateFlowRequest, MessageContext, Task> handler)
        {
            _bus.SubscribeAsync(handler);
        }
    }
}
