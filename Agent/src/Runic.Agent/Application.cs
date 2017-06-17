using NLog;
using Runic.Agent.Services;
using Runic.Agent.ThreadManagement;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent
{
    public class Application : IApplication
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IMessagingService _messagingService;

        public Application(IMessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        public async Task RunApplicationAsync(CancellationToken ct = default(CancellationToken))
        {
            //run the messaging service
            await _messagingService.RunServiceAsync(ct);
        }
    }
}
