using NLog;
using Runic.Agent.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent
{
    public class Application : IApplication
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IMessagingService _messagingService;
        private readonly IHandlerRegistry _handlerRegistry;

        public Application(IMessagingService messagingService, IHandlerRegistry handlerRegistry)
        {
            _messagingService = messagingService;
            _handlerRegistry = handlerRegistry;
        }

        public async Task RunApplicationAsync(CancellationToken ct = default(CancellationToken))
        {
            _handlerRegistry.RegisterMessageHandlers(ct);
            //run the messaging service
            await _messagingService.RunServiceAsync(ct);
        }
    }
}
