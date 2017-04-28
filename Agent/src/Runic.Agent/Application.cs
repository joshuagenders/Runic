using NLog;
using Runic.Agent.Messaging;
using Runic.Agent.Service;
using System.Threading;
using System.Threading.Tasks;

namespace Runic.Agent
{
    public class Application : IApplication
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IAgentService _agentService;
        private readonly IMessagingService _messagingService;

        public Application(IAgentService agentService, IMessagingService messagingService)
        {
            _agentService = agentService;
            _messagingService = messagingService;
        }

        public async Task Run(CancellationToken ct = default(CancellationToken))
        {
            var serviceTasks = new Task[]
            {
                _messagingService.Run(ct),
                _agentService.Run(ct)
            };
            await Task.WhenAll(serviceTasks);
        }
    }
}
