using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.Services.Interfaces
{
    public interface IDatetimeService
    {
        DateTime Now { get; }
        Task WaitUntil(int durationMilliseconds, CancellationToken ctx = default(CancellationToken));
    }
}
