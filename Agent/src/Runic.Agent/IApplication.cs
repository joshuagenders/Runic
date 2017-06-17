using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent
{
    public interface IApplication
    {
        Task RunApplicationAsync(CancellationToken ct = default(CancellationToken));
    }
}
