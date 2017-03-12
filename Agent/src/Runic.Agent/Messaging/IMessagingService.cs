using System;
using System.Threading.Tasks;
using RawRabbit.Context;
using Runic.Framework.Models;

namespace Runic.Agent.Messaging
{
    public interface IMessagingService
    {
        void RegisterThreadLevelHandler(Func<SetThreadLevelRequest, MessageContext, Task> handler);
        void RegisterFlowUpdateHandler(Func<AddUpdateFlowRequest, MessageContext, Task> handler);
    }
}