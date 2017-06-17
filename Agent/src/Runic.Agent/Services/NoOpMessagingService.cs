using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Services
{
    public class NoOpMessagingService : IMessagingService
    {
        public void RegisterMessageHandler<T>(Func<T, Task> handler)
        {
        }

        public async Task RunServiceAsync(CancellationToken ct)
        {
            await Task.Run(() =>
            {
                var mre = new ManualResetEventSlim(false);
                ct.Register(() => mre.Set());
                mre.Wait();
            }, ct);
        }
    }
}
