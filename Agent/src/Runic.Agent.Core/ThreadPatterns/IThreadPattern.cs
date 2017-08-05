using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.ThreadPatterns
{
    public interface IThreadPattern
    {
        void RegisterThreadChangeHandler(Action<int> callback);
        Task StartPatternAsync(CancellationToken ctx = default(CancellationToken));
        int GetMaxDurationSeconds();
        int GetMaxThreadCount();
    }
}
