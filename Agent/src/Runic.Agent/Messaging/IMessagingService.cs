using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Messaging
{
    public interface IMessagingService
    {
        Task RunServiceAsync(CancellationToken ct);
        void RegisterMessageHandler<T>(Action<T> handler) where T : class;
        void PublishMessage<T>(T message);
    }
}