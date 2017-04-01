using System.Threading;
using System.Threading.Tasks;
using Runic.Framework.Models;

namespace Runic.Agent.Service
{
    public interface IAgentService
    {
        Task Run(CancellationToken ct);
        Task SetThreadLevel(SetThreadLevelRequest request, CancellationToken ct);
    }
}
