using Runic.Agent.Framework.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.ThreadPatterns
{
    public interface IPopulationPatternController
    {
        void AddPopulationPattern(string id, Journey flow, IPopulationPattern pattern, CancellationToken ctx);
        Task Stop(string id, CancellationToken ctx);
        Task StopAll(CancellationToken ctx);
        Task Run(CancellationToken ctx);
    }
}
