using Microsoft.Extensions.Logging;
using Runic.Agent.Worker.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent.Worker
{
    public class Application : IApplication
    {
        private static readonly ILogger _logger = new LoggerFactory().CreateLogger(nameof(Application));
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
