using Runic.Framework.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Worker.Messaging
{
    public interface IMessagingService
    {
        Task RunServiceAsync(CancellationToken ctx = default(CancellationToken));
        void PublishMessage<T>(T message);
        void RegisterMessageHandler(Func<SetThreadLevelRequest, Task> handler);
        void RegisterMessageHandler(Func<AddUpdateFlowRequest, Task> handler);
        void RegisterMessageHandler(Func<ConstantFlowExecutionRequest, Task> handler);
        void RegisterMessageHandler(Func<GraphFlowExecutionRequest, Task> handler);
        void RegisterMessageHandler(Func<GradualFlowExecutionRequest, Task> handler);
    }
}