using System.Threading;

namespace Runic.Agent.Core.WorkGenerator
{
    public interface IConsumer<T>
    {
        void EnqueueTask(T workItem);
        void ProcessQueue(CancellationToken ctx);
        void Close();
        bool Closed { get; }
        int Count { get; }
    }
}
