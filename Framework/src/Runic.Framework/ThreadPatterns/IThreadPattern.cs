using System;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Framework.ThreadPatterns
{
    public interface IThreadPattern
    {
        void RegisterThreadChangeHandler(Action<int> callback);
        Task Start(CancellationToken ct);
    }
}
