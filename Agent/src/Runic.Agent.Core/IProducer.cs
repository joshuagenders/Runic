using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Core
{
    public interface IProducer<T>
    {
        void AddUpdateWorkItem(string id, T item);
        T GetWorkItem(string id);
        Task ProduceWorkItems(CancellationToken ctx);
    }
}
