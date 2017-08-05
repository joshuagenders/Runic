using Runic.Agent.Worker.Messaging;
using Runic.Framework.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Worker.Services
{
    public class SqsMessagingService : IMessagingService
    {
        public void PublishMessage<T>(T message)
        {
            throw new NotImplementedException();
        }

        public void RegisterMessageHandler(Func<SetThreadLevelRequest, Task> handler)
        {
            throw new NotImplementedException();
        }

        public void RegisterMessageHandler(Func<AddUpdateFlowRequest, Task> handler)
        {
            throw new NotImplementedException();
        }

        public void RegisterMessageHandler(Func<ConstantFlowExecutionRequest, Task> handler)
        {
            throw new NotImplementedException();
        }

        public void RegisterMessageHandler(Func<GraphFlowExecutionRequest, Task> handler)
        {
            throw new NotImplementedException();
        }

        public void RegisterMessageHandler(Func<GradualFlowExecutionRequest, Task> handler)
        {
            throw new NotImplementedException();
        }

        public Task RunServiceAsync(CancellationToken ctx = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
