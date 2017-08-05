using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Api.Messaging
{
    public interface IMessagingClient
    {
        Task PublishMessageAsync<T>(T message, CancellationToken ctx = default(CancellationToken));
    }
}
