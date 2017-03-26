using System.Threading;
using System.Threading.Tasks;
using Runic.Agent.Messaging;
using Runic.Framework.Models;

namespace Runic.Agent.Service
{
    public interface IAgentService
    {
        Task Run(IMessagingService service, CancellationToken ct);
        Task SetThreadLevel(SetThreadLevelRequest request, CancellationToken ct);
    }
}
