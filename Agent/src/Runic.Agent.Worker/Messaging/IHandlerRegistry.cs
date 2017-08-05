using System.Threading;

namespace Runic.Agent.Worker.Messaging
{
    public interface IHandlerRegistry
    {
        void RegisterMessageHandlers(CancellationToken ctx = default(CancellationToken));
    }
}
