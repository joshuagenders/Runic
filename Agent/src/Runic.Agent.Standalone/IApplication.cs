using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Standalone
{
    public interface IApplication
    {
        Task Run(CancellationToken ctx);
    }
}