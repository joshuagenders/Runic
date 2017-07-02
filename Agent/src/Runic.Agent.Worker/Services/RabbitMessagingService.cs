using System;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.Worker.Messaging;

namespace Runic.Agent.Worker.Services
{
    public class RabbitMessagingService : IMessagingService
    {
        public void PublishMessage<T>(T message)
        {
            throw new NotImplementedException();
        }

        public void RegisterMessageHandler<T>(Action<T> handler) where T : class
        {
            throw new NotImplementedException();
        }

        public Task RunServiceAsync(CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
