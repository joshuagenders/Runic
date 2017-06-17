using System.Threading;

namespace Runic.Agent.Messaging
{
    public interface IHandlerRegistry
    {
        void RegisterMessageHandlers(CancellationToken ct = default(CancellationToken));
    }
}
