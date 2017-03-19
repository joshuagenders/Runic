using System;
using System.Threading.Tasks;
using RawRabbit.Context;
using Runic.Framework.Models;

namespace Runic.Agent.Messaging
{
    public class NoOpMessagingService : IMessagingService
    {
        public void RegisterFlowUpdateHandler(Func<AddUpdateFlowRequest, MessageContext, Task> handler)
        {
        }

        public void RegisterThreadLevelHandler(Func<SetThreadLevelRequest, MessageContext, Task> handler)
        {
        }
    }
}
