using Runic.Agent.Framework.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Runic.Agent.TestHarness.Services
{
    public interface IPerson
    {
        Task PerformJourneyAsync(Journey journey, CancellationToken ctx = default(CancellationToken));
        void SetAttributes(Dictionary<string, string> attributes);
    }
}
