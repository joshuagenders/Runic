using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Aws
{
    public interface IApplication
    {
        Task RunApplicationAsync(CancellationToken ctx = default(CancellationToken));
    }
}
