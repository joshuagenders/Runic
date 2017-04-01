using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Messaging
{
    public class NoOpMessagingService : IMessagingService
    {
        public async Task Run(CancellationToken ct)
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
