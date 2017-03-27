using System;
using System.Threading.Tasks;
using RawRabbit.Context;
using Runic.Framework.Models;

namespace Runic.Agent.Messaging
{
    public class NoOpMessagingService : IMessagingService
    {
        public void RegisterConstantFlowHandler(Func<ConstantFlowExecutionRequest, MessageContext, Task> handler)
        {
        }

        public void RegisterFlowUpdateHandler(Func<AddUpdateFlowRequest, MessageContext, Task> handler)
        {
        }

        public void RegisterGradualFlowHandler(Func<GradualFlowExecutionRequest, MessageContext, Task> handler)
        {
        }

        public void RegisterGraphFlowHandler(Func<GraphFlowExecutionRequest, MessageContext, Task> handler)
        {
        }

        public void RegisterThreadLevelHandler(Func<SetThreadLevelRequest, MessageContext, Task> handler)
        {
        }
    }
}
