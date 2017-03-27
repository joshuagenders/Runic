using System;
using System.Threading.Tasks;
using RawRabbit.Context;
using Runic.Framework.Models;

namespace Runic.Agent.Messaging
{
    //todo implement proper interface that accepts multuple messaging frameworks
    public interface IMessagingService
    {
        void RegisterThreadLevelHandler(Func<SetThreadLevelRequest, MessageContext, Task> handler);
        void RegisterFlowUpdateHandler(Func<AddUpdateFlowRequest, MessageContext, Task> handler);
        void RegisterGraphFlowHandler(Func<GraphFlowExecutionRequest, MessageContext, Task> handler);
        void RegisterConstantFlowHandler(Func<ConstantFlowExecutionRequest, MessageContext, Task> handler);
        void RegisterGradualFlowHandler(Func<GradualFlowExecutionRequest, MessageContext, Task> handler);
    }
}