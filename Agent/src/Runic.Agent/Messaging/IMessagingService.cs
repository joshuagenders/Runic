using Runic.Core.Models;
using System.Threading.Tasks;

namespace Runic.Agent
{
    public interface IMessagingService
    {
        Task<ExecutionRequest> ReceiveRequest();
    }
}
