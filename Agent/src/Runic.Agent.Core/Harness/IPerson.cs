using Runic.Agent.Core.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Runic.Agent.Core.Harness
{
    public interface IPerson
    {
        Task PerformJourneyAsync(Journey journey, CancellationToken ctx = default(CancellationToken));
        void SetAttributes(Dictionary<string, string> attributes);
    }
}
