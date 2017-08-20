using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core.Services
{
    public class DateTimeService : IDatetimeService
    {
        public DateTime Now => DateTime.Now;
        public async Task WaitMilliseconds(int durationMilliseconds, CancellationToken ctx = default(CancellationToken))
        {
            var start = Now;
            await Task.Run(() => ctx.WaitHandle.WaitOne(TimeSpan.FromMilliseconds(durationMilliseconds)), ctx);
        }
    }
}
