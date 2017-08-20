using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.Services
{
    public interface IDatetimeService
    {
        DateTime Now { get; }
        Task WaitMilliseconds(int durationMilliseconds, CancellationToken ctx = default(CancellationToken));
    }
}
