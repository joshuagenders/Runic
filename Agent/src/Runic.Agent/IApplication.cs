using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent
{
    public interface IApplication
    {
        Task Run(CancellationToken ct = default(CancellationToken));
    }
}
