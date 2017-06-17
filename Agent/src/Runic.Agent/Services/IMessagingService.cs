using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Services
{
    public interface IMessagingService
    {
        Task RunServiceAsync(CancellationToken ct);
        void RegisterMessageHandler<T>(Func<T, Task> handler);
    }
}