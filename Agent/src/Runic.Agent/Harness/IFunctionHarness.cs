using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Harness
{
    public interface IFunctionHarness
    {
        Task Execute(string functionName, CancellationToken ct);
        void Bind(object functionInstance);
    }
}
