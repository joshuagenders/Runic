using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Messaging
{
    public interface IMessagingService
    {
        Task Run(CancellationToken ct);
    }
}