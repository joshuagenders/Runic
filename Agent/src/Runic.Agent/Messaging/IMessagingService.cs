using System.Threading.Tasks;
using Runic.Core.Models;

namespace Runic.Agent.Messaging
{
    public interface IMessagingService
    {
        Task<ExecutionRequest> ReceiveRequest();
    }
}