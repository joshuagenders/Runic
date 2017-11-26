using Runic.Agent.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.Harness
{
    public interface IPerson
    {
        Task PerformJourneyAsync(Journey journey, CancellationToken ctx = default(CancellationToken));
    }
}
