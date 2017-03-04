using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.Messaging;
using Runic.Core.Models;

namespace Runic.Agent.Service
{
    public interface IAgentService
    {
        Task Run(IMessagingService messagingService, CancellationToken ct);
        Task AddUpdateFlow(Flow flow, CancellationToken ct);
        Task SetThreadLevel(SetThreadLevelRequest request, CancellationToken ct);
    }
}
