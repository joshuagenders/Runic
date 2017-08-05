using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Worker
{
    public interface IApplication
    {
        Task RunApplicationAsync(CancellationToken ctx = default(CancellationToken));
    }
}
