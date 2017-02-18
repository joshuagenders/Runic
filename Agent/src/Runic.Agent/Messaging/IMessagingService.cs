using Runic.Core.Messaging;
using System.Threading.Tasks;

namespace Runic.Agent
{
    public interface IMessagingService
    {
        Task<ExecutionRequest> ReceiveRequest();
    }
}
