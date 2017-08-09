using Runic.Framework.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.Services.Interfaces
{
    public interface IRunnerService
    {
        Task ExecuteFlowAsync(Flow flow, CancellationToken ctx = default(CancellationToken));
    }
}
