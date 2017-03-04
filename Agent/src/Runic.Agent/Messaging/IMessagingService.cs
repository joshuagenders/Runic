using System;
using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.DTO;
using Runic.Core.Models;

namespace Runic.Agent.Messaging
{
    public interface IMessagingService
    {
        void RegisterThreadLevelHandler<T>(string subscriptionId, Func<SetThreadLevelRequest, Task> handler);
        void RegisterFlowUpdateHandler<T>(string subscriptionId, Func<AddUpdateFlowRequest, Task> handler);
        Task<SetThreadLevelRequest> ReceiveThreadLevelRequest(CancellationToken ct);
        Task<AddUpdateFlowRequest> ReceiveUpdateFlowRequest(CancellationToken ct);
    }
}