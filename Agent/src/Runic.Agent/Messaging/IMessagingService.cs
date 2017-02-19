using System.Threading.Tasks;
using Runic.Core.Models;

namespace Runic.Agent
{
    public interface IMessagingService
    {
        Task<ExecutionRequest> ReceiveRequest();
    }
}